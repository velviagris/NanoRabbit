using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NanoRabbit;
using NanoRabbit.DependencyInjection;
using System.Collections.Concurrent;
using System.Text;

var builder = Host.CreateApplicationBuilder();

// must be injected before IRabbitHelper's injection.
builder.Services.AddSingleton<IRedisConnectionFactory>(provider =>
{
    var connStr = provider.GetRequiredService<IConfiguration>().GetSection("DbConfig").GetSection(nameof(RedisConfig)).Get<RedisConfig>().DbConnStr;
    return new RedisConnectionFactory(connStr);
});

builder.Services.AddKeyedRabbitHelper("test", builder =>
{
    builder.SetHostName("localhost")
        .SetPort(5672)
        .SetVirtualHost("test")
        .SetUserName("admin")
        .SetPassword("admin")
        .AddProducerOption(producer =>
        {
            producer.ProducerName = "FooProducer";
            producer.ExchangeName = "amq.topic";
            producer.RoutingKey = "foo.key";
            producer.Type = ExchangeType.Topic;
        });
});

builder.Services.AddKeyedRabbitHelper("default", builder =>
{
    builder.SetHostName("localhost")
        .SetPort(5672)
        .SetVirtualHost("/")
        .SetUserName("admin")
        .SetPassword("admin")
        .UseAsyncConsumer(true) // set UseAsyncConsumer to true
        .AddConsumerOption(consumer =>
        {
            consumer.ConsumerName = "FooConsumer";
            consumer.QueueName = "foo-queue";
        });
})
.AddKeyedRabbitAsyncHandler<FooQueueHandler>("default");


var host = builder.Build();
await host.RunAsync();

public class FooQueueHandler : IAsyncMessageHandler
{
    private readonly IRedisConnectionFactory _connFactory;
    private readonly IRabbitHelper _rabbitHelper;
    private readonly ConcurrentQueue<string> _queue;

    public FooQueueHandler(IServiceProvider serviceProvider,
        IRedisConnectionFactory connFactory
        )
    {
        _rabbitHelper = serviceProvider.GetRabbitHelper("test");
        _connFactory = connFactory;
        _queue = new ConcurrentQueue<string>();

        Task.Run(() => ProcessQueueAsync());
    }
    
    public async Task<bool> HandleMessageAsync(byte[] messageBody, string? routingKey = null, string? correlationId = null)
    {
        var message = Encoding.UTF8.GetString(messageBody);
        Console.WriteLine($"[x] Received from foo-queue: {message}");

        _queue.Enqueue(message);

        Console.WriteLine($"Message {message} enqueued.");
        return true;
    }

    private async Task ProcessQueueAsync()
    {
        while (true)
        {
            var batch = new List<string>();
            while (_queue.TryDequeue(out var message))
            {
                batch.Add(message);

                // Process 10 messages each times
                if (batch.Count >= 10)
                {
                    break;
                }
            }

            if (batch.Count > 0)
            {
                await ProcessBatchAsync(batch);
            }

            // Set a small delay to avoid high usage of CPU
            await Task.Delay(100);
        }
    }

    private async Task ProcessBatchAsync(List<string> batch)
    {
        // Save batch of messages to redis
        var redisConn = _connFactory.GetConnection();
        var redisDb = redisConn.GetDatabase();

        var tasks = batch.Select(async message => {
            await redisDb.StringSetAsync(Guid.NewGuid().ToString(), message);
            _rabbitHelper.Publish("FooProducer", message);
        }).ToList();
        await Task.WhenAll(tasks);

        //foreach (var message in batch)
        //{
        //    await redisDb.StringSetAsync($"{Guid.NewGuid().ToString()}-{message}", message);
        //    _rabbitHelper.Publish("FooProducer", message);
        //}

        Console.WriteLine($"[x] Processed batch of {batch.Count} messages");
    }
}
