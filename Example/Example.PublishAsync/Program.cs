using Microsoft.Extensions.Logging;
using NanoRabbit;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger("RabbitHelper");

var rabbitHelper = new RabbitHelper(rabbitConfig: new RabbitConfiguration
{
    HostName = "localhost",
    Port = 5672,
    VirtualHost = "/",
    UserName = "admin",
    Password = "admin",
    Producers = new List<ProducerOptions> { new ProducerOptions {
            ProducerName = "FooProducer",
            ExchangeName = "amq.topic",
            RoutingKey = "foo.key",
            Type= ExchangeType.Topic
        }
    }
}, logger);

await rabbitHelper.PublishAsync<string>("FooProducer", "Hello from NanoRabbit");

await rabbitHelper.PublishBatchAsync<string>("FooProducer", new List<string> { "Test 1", "Test 2" });

Console.WriteLine(" Press [enter] to exit.");
