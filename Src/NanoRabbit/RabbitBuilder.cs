namespace NanoRabbit;

/// <summary>
/// RabbitConfiguration Builder.
/// </summary>
public class RabbitConfigurationBuilder
{
    private readonly RabbitConfiguration _rabbitConfiguration;

    /// <summary>
    /// RabbitConfiguration constructor.
    /// </summary>
    public RabbitConfigurationBuilder()
    {
        _rabbitConfiguration = new RabbitConfiguration
        {
            Producers = new List<ProducerOptions>(),
            Consumers = new List<ConsumerOptions>()
        };
    }

    /// <summary>
    /// Set hostname of RabbitMQ connection.
    /// </summary>
    /// <param name="hostName"></param>
    public RabbitConfigurationBuilder SetHostName(string hostName)
    {
        _rabbitConfiguration.HostName = hostName;

        return this;
    }

    /// <summary>
    /// Set port of RabbitMQ amqp connection.
    /// </summary>
    /// <param name="port"></param>
    public RabbitConfigurationBuilder SetPort(int port)
    {
        _rabbitConfiguration.Port = port;

        return this;
    }

    /// <summary>
    /// Set virtual host of RabbitMQ connection.
    /// </summary>
    /// <param name="virtualHost"></param>
    public RabbitConfigurationBuilder SetVirtualHost(string virtualHost)
    {
        _rabbitConfiguration.VirtualHost = virtualHost;

        return this;
    }

    /// <summary>
    /// Set username of RabbitMQ connection.
    /// </summary>
    /// <param name="userName"></param>
    public RabbitConfigurationBuilder SetUserName(string userName)
    {
        _rabbitConfiguration.UserName = userName;

        return this;
    }

    /// <summary>
    /// Set password of RabbitMQ connection.
    /// </summary>
    /// <param name="password"></param>
    public RabbitConfigurationBuilder SetPassword(string password)
    {
        _rabbitConfiguration.Password = password;

        return this;
    }

    /// <summary>
    /// Set to true will enable an asynchronous consumer dispatcher. Defaults to false.
    /// </summary>
    /// <param name="useAsyncConsumer"></param>
    public RabbitConfigurationBuilder UseAsyncConsumer(bool useAsyncConsumer)
    {
        _rabbitConfiguration.UseAsyncConsumer = useAsyncConsumer;

        return this;
    }

    /// <summary>
    /// Connect to RabbitMQ using TLS.
    /// </summary>
    /// <param name="config"></param>
    public RabbitConfigurationBuilder UseTLS(TLSConfig config)
    {
        _rabbitConfiguration.TLSConfig = config;

        return this;
    }

    /// <summary>
    /// Set ClientProvidedName for connections.
    /// </summary>
    /// <param name="connectionName"></param>
    public RabbitConfigurationBuilder SetConnectionName(string connectionName)
    {
        _rabbitConfiguration.ConnectionName = connectionName;

        return this;
    }

    /// <summary>
    /// Add a producer to RabbitMQ connection.
    /// </summary>
    /// <param name="configureProducer"></param>
    public RabbitConfigurationBuilder AddProducerOption(Action<ProducerOptions> configureProducer)
    {
        var options = new ProducerOptions();
        configureProducer(options);
        if (_rabbitConfiguration.Producers != null)
        {
            if (_rabbitConfiguration.Producers.Any(x=>x.ProducerName == options.ProducerName))
            {
                throw new Exception($"Producer '{options.ProducerName}' already registered in IRabbitHelper.");
            }
            _rabbitConfiguration.Producers.Add(options);
        }

        return this;
    }

    /// <summary>
    /// Add a consumer to RabbitMQ connection.
    /// </summary>
    /// <param name="configureConsumer"></param>
    public RabbitConfigurationBuilder AddConsumerOption(Action<ConsumerOptions> configureConsumer)
    {
        var options = new ConsumerOptions();
        configureConsumer(options);
        if (_rabbitConfiguration.Consumers != null)
        {
            if (_rabbitConfiguration.Consumers.Any(x => x.ConsumerName == options.ConsumerName))
            {
                throw new Exception($"Consumer '{options.ConsumerName}' already registered in IRabbitHelper.");
            }
            _rabbitConfiguration.Consumers.Add(options);
        }

        return this;
    }

    /// <summary>
    /// Build RabbitConfiguration.
    /// </summary>
    /// <returns></returns>
    public RabbitConfiguration Build()
    {
        return _rabbitConfiguration;
    }
}
