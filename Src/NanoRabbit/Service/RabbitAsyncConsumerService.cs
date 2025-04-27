using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NanoRabbit.Service
{
    public class RabbitAsyncConsumerService<TConfiguration> : BackgroundService
    where TConfiguration : RabbitConfiguration
    {
        private readonly ILogger<RabbitAsyncConsumerService<TConfiguration>> _logger;
        private readonly TConfiguration _configuration;
        private readonly ConsumerOptions _options;
        private readonly IServiceProvider _serviceProvider;
        private IConnection? _connection;
        private IModel? _channel;
        private AsyncEventingBasicConsumer? _consumer;
        private string _consumerTag = string.Empty;
        private readonly string _instanceId; // Used to distinguish between different consumer instances

        public RabbitAsyncConsumerService(
            ILogger<RabbitAsyncConsumerService<TConfiguration>> logger,
            TConfiguration configuration,
            string consumerName,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            if (_configuration.Consumers == null) throw new ArgumentNullException(nameof(_configuration.Consumers));
            _options = _configuration.Consumers.FirstOrDefault(x => x.ConsumerName == consumerName) ??
                       throw new ArgumentNullException($"Could not find consumer options: {consumerName}");

            _serviceProvider = serviceProvider;
            _instanceId = $"{_options.ConsumerName}-{Guid.NewGuid().ToString("N")[..6]}"; // create a short instance id
            _logger.LogInformation("RabbitMQ Consumer Service [{InstanceId}] Initializing...", _instanceId);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service [{InstanceId}] Starting, Subscribing queue: {QueueName}", _instanceId,
                _options.QueueName);
            stoppingToken.Register(() => _logger.LogInformation("RabbitMQ Consumer Service [{InstanceId}] Stopping...", _instanceId));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        Connect(stoppingToken); // Reconnect
                    }

                    // Keep ExecuteAsync running, the actual work is done by the EventingBasicConsumer's event handler.
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // BackgroundService canceled, exit normally
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMQ Consumer Service [{InstanceId}] An unhandled exception occurred. Will retry after 5 seconds...", _instanceId);
                    // Close old resources that may exist
                    CloseConnection();
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("RabbitMQ Consumer Service [{InstanceId}] Stopped.", _instanceId);
            CloseConnection();
        }

        private void Connect(CancellationToken stoppingToken)
        {
            if (_connection != null && _connection.IsOpen) return; // Check if connected

            CloseConnection(); // Close old resources that may exist

            var factory = new ConnectionFactory()
            {
                HostName = _configuration.HostName,
                Port = _configuration.Port,
                UserName = _configuration.UserName,
                Password = _configuration.Password,
                VirtualHost = _configuration.VirtualHost,
                DispatchConsumersAsync = true, // Enable an asynchronous consumer dispatcher
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            try
            {
                _logger.LogInformation("RabbitMQ Consumer [{InstanceId}] Connectiong to {HostName}:{Port}...", _instanceId,
                    factory.HostName, factory.Port);
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Event handling for connections and channels (optional, for logging or special handling)
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _channel.CallbackException += OnChannelCallbackException;
                _channel.ModelShutdown += OnChannelModelShutdown;

                _logger.LogInformation("RabbitMQ Consumer [{InstanceId}] Connected, Channel created.", _instanceId);
                
                _channel.BasicQos(prefetchSize: 0, prefetchCount: _options.PrefetchCount, global: false);
                _logger.LogInformation("RabbitMQ Consumer [{InstanceId}] QoS Set PrefetchCount={PrefetchCount}", _instanceId,
                    _options.PrefetchCount);
                
                if (_options.DeclareQueue)
                {
                    _logger.LogInformation("RabbitMQ Consumer [{InstanceId}] Declaring Queue '{QueueName}'...", _instanceId,
                        _options.QueueName);
                    _channel.QueueDeclare(queue: _options.QueueName,
                        durable: _options.QueueDurable,
                        exclusive: _options.QueueExclusive,
                        autoDelete: _options.QueueAutoDelete,
                        arguments: _options.QueueArguments);
                }

                _consumer = new AsyncEventingBasicConsumer(_channel);
                _consumer.Received += async (model, ea) => { await HandleMessageReceived(ea, stoppingToken); };
                
                _consumerTag = _channel.BasicConsume(queue: _options.QueueName, autoAck: _options.AutoAck,
                    consumer: _consumer);
                _logger.LogInformation("RabbitMQ Consumer [{InstanceId}] Subscribing to '{QueueName}'，ConsumerTag: {ConsumerTag}",
                    _instanceId, _options.QueueName, _consumerTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ Consumer [{InstanceId}] Connection or Setup Failure.", _instanceId);
                CloseConnection();
                throw; // Throw an exception upwards for ExecuteAsync's retry logic to handle
            }
        }

        private async Task<bool> HandleMessageReceived(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
        {
            var messageBody = ea.Body.ToArray();
            var deliveryTag = ea.DeliveryTag;
            var correlationId = ea.BasicProperties?.CorrelationId;
            bool processedSuccessfully = false;

            _logger.LogDebug(
                "RabbitMQ Consumer [{InstanceId}] Received DeliveryTag={DeliveryTag}, CorrelationId='{CorrelationId}'",
                _instanceId, deliveryTag, correlationId);

            // Create a new Dependency Injection Scope for each message process
            // This is essential for working with Scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                var messageHandler = scope.ServiceProvider.GetRequiredKeyedService<IAsyncMessageHandler>(_options.HandlerName);

                if (messageHandler == null)
                {
                    _logger.LogError("RabbitMQ Consumer [{InstanceId}] Unable to resolve IMessageHandler service. The message will not be processed.", _instanceId);
                    
                    try
                    {
                        _channel?.BasicNack(deliveryTag, false, true);
                    }
                    catch (Exception nackEx)
                    {
                        _logger.LogError(nackEx, "Nack failed.");
                    }

                    return processedSuccessfully;
                }

                try
                {
                    // Handle message
                    processedSuccessfully =
                        await messageHandler.HandleMessageAsync(messageBody, ea.RoutingKey, correlationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "RabbitMQ Consumer [{InstanceId}] An exception occurred when calling IMessageHandler. DeliveryTag={DeliveryTag}, CorrelationId='{CorrelationId}'",
                        _instanceId, deliveryTag, correlationId);
                    processedSuccessfully = false; // Mark as failed
                }
            } // Scope Dispose

            // Acknowledge or reject the message based on the result (if not AutoAck)
            if (!_options.AutoAck)
            {
                try
                {
                    if (processedSuccessfully)
                    {
                        _channel?.BasicAck(deliveryTag, multiple: false);
                        _logger.LogDebug("RabbitMQ Consumer [{InstanceId}] Ack Succeeded. DeliveryTag={DeliveryTag}",
                            _instanceId, deliveryTag);
                    }
                    else
                    {
                        _channel?.BasicNack(deliveryTag, multiple: false, requeue: false);
                        _logger.LogWarning("RabbitMQ Consumer [{InstanceId}] Nack Failed. DeliveryTag={DeliveryTag}",
                            _instanceId, deliveryTag);
                        // TODO: Consider sending failed messages to a dead message queue or logging to a database
                        processedSuccessfully = false; 
                    }
                }
                catch (Exception ackNackEx)
                {
                    _logger.LogError(ackNackEx,
                        "RabbitMQ Consumer [{InstanceId}] Ack/Nack failed. DeliveryTag={DeliveryTag}", _instanceId,
                        deliveryTag);
                    // TODO: Consider sending failed messages to a dead message queue or logging to a database
                    processedSuccessfully = false; 
                }
            }
            return processedSuccessfully;
        }

        private void CloseConnection()
        {
            if (_channel != null && _channel.IsOpen)
            {
                try
                {
                    if (!string.IsNullOrEmpty(_consumerTag))
                    {
                        _channel.BasicCancel(_consumerTag); // Stop consuming
                    }

                    _channel.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error [{InstanceId}] occurred while closing RabbitMQ channel.", _instanceId);
                }

                _channel = null;
            }

            if (_connection != null && _connection.IsOpen)
            {
                try
                {
                    _connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error [{InstanceId}] occurred while closing RabbitMQ connection.", _instanceId);
                }

                _connection = null;
            }

            _logger.LogInformation("RabbitMQ connection and channel closed [{InstanceId}].", _instanceId);
        }

        public override void Dispose()
        {
            _logger.LogInformation("RabbitMQ Consumer Service [{InstanceId}] Disposing...", _instanceId);
            CloseConnection();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
        
        private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogWarning("RabbitMQ Connection Closed [{InstanceId}]. Reason: {Reason}", _instanceId, e.ReplyText);
            // After the connection is closed, the loop in ExecuteAsync tries to reconnect
        }

        private void OnChannelModelShutdown(object? sender, ShutdownEventArgs e)
        {
            _logger.LogWarning("RabbitMQ Channel Close [{InstanceId}]. Reason: {Reason}", _instanceId, e.ReplyText);
        }

        private void OnChannelCallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            _logger.LogError(e.Exception, "An exception occurs in the channel callback [{InstanceId}].", _instanceId);
        }
    }
}