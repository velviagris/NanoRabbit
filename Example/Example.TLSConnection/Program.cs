// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using NanoRabbit;
using System.Security.Authentication;

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
    TLSConfig = new TLSConfig
    {
        Enabled = true,
        CertPath = "/path/to/client_key.p12",
        CertPassphrase = "MySecretPassword",
        Version = SslProtocols.Ssl2
    },
    Producers = new List<ProducerOptions> { new ProducerOptions {
            ProducerName = "FooProducer",
            ExchangeName = "amq.topic",
            RoutingKey = "foo.key"
        }
    },
    Consumers = new List<ConsumerOptions> { new ConsumerOptions {
            ConsumerName= "FooConsumer",
            QueueName = "foo-queue"
        }
    }
}, logger);

rabbitHelper.Publish<string>("FooProducer", "Hello from NanoRabbit");

Console.WriteLine(" Press [enter] to exit.");

while (true)
{
    rabbitHelper.AddConsumer("FooConsumer", message =>
    {
        Console.WriteLine(message);
    });

    if (Console.ReadLine() == "")
    {
        break;
    }
}
