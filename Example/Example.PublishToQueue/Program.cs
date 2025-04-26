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
    VirtualHost = "bus",
    UserName = "admin",
    Password = "admin",
    Producers = new List<ProducerOptions> { new ProducerOptions {
            ProducerName = "FooProducer",
            RoutingKey = "no-key-queue",
        }
    }
}, logger);

rabbitHelper.Publish<string>("FooProducer", "Hello from NanoRabbit");

Console.WriteLine(" Press [enter] to exit.");