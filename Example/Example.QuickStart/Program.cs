using Microsoft.Extensions.Logging;
using NanoRabbit;
using NanoRabbit.Service;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger("RabbitHelper");

var rabbitConfig = new RabbitConfiguration
{
    HostName = "localhost",
    Port = 5672,
    VirtualHost = "/",
    UserName = "admin",
    Password = "admin",
    Producers = new List<ProducerOptions>
    {
        new ProducerOptions
        {
            ProducerName = "FooProducer",
            ExchangeName = "amq.topic",
            RoutingKey = "foo.key",
            Type = ExchangeType.Topic
        }
    },
    Consumers = new List<ConsumerOptions>
    {
        new ConsumerOptions
        {
            ConsumerName = "FooConsumer",
            QueueName = "foo-queue"
        }
    }
};

var rabbitHelper = new RabbitHelper(rabbitConfig, logger);

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
