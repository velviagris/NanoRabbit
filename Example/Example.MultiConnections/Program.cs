using System.Text;
using Example.MultiConnections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NanoRabbit;
using NanoRabbit.DependencyInjection;
using NanoRabbit.Service;

var builder = Host.CreateApplicationBuilder();



var fooConfig = new RabbitConfiguration
{
    HostName = "localhost",
    Port = 5672,
    UserName = "admin",
    Password = "admin",
    VirtualHost = "/",
    UseAsyncConsumer = false,
    ConnectionName = "FooConnection",
    Consumers = new List<ConsumerOptions>()
    {
        new ConsumerOptions
        {
            ConsumerName = "FooConsumer",
            HandlerIdentifier = "FooQueueHandler",
            QueueName = "foo-queue",
            AutomaticRecoveryEnabled = true,
            PrefetchSize = 0,
            PrefetchCount = 10,
            AutoAck = false,
            ConsumerCount = 2
        }
    }
};

var barConfig = new RabbitConfiguration
{
    HostName = "localhost",
    Port = 5672,
    UserName = "admin",
    Password = "admin",
    VirtualHost = "/",
    UseAsyncConsumer = false,
    ConnectionName = "BarConnection",
    Consumers = new List<ConsumerOptions>()
    {
        new ConsumerOptions
        {
            ConsumerName = "BarConsumer",
            HandlerIdentifier = "BarQueueHandler",
            QueueName = "bar-queue",
            AutomaticRecoveryEnabled = true,
            PrefetchSize = 0,
            PrefetchCount = 10,
            AutoAck = false,
        }
    }
};

// builder.Services.AddKeyedScoped<IMessageHandler, FooQueueHandler>(nameof(FooQueueHandler));
// builder.Services.AddKeyedScoped<IMessageHandler, BarQueueHandler>("BarQueueHandler");

builder.Services.AddKeyedRabbitHelper("DefaultRabbitHelper", builder =>
{
    builder.SetHostName("localhost")
        .SetPort(5672)
        .SetVirtualHost("/")
        .SetUserName("admin")
        .SetPassword("admin")
        .SetConnectionName("FooConnection")
        .AddProducerOption(producer =>
        {
            producer.ProducerName = "FooProducer";
            producer.ExchangeName = "amq.topic";
            producer.RoutingKey = "foo.key";
            producer.Type = ExchangeType.Topic;
        })
        .AddConsumerOption(consumer =>
        {
            consumer.ConsumerName = "FooConsumer";
            consumer.QueueName = "foo-queue";
            consumer.HandlerIdentifier = "FooQueueHandler";
        });
});

builder.Services.AddKeyedRabbitHelper("TestRabbitHelper", builder =>
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
    })
    .AddConsumerOption(consumer =>
    {
        consumer.ConsumerName = "BarConsumer";
        consumer.QueueName = "foo-queue";
        consumer.HandlerIdentifier = "BarQueueHandler";
    });
});

builder.Services.AddKeyedRabbitHandler<FooQueueHandler>(nameof(FooQueueHandler));
builder.Services.AddKeyedRabbitHandler<BarQueueHandler>(nameof(BarQueueHandler));

builder.Services.AddKeyedRabbitConsumerService("DefaultRabbitHelper");
builder.Services.AddKeyedRabbitConsumerService("TestRabbitHelper");

builder.Services.AddHostedService<DefaultPublishService>();
builder.Services.AddHostedService<TestPublishService>();

var host = builder.Build();
await host.RunAsync();

public class FooQueueHandler : IMessageHandler
{
    public bool HandleMessage(byte[] messageBody, string? routingKey = null,
        string? correlationId = null)
    {
        var message = Encoding.UTF8.GetString(messageBody);
        Console.WriteLine($"[x] Received from foo-queue: {message}");
        Task.Delay(1000);
        Console.WriteLine("[x] Done");
        return true;
    }
}

public class BarQueueHandler : IMessageHandler
{
    public bool HandleMessage(byte[] messageBody, string? routingKey = null,
        string? correlationId = null)
    {
        var message = Encoding.UTF8.GetString(messageBody);
        Console.WriteLine($"[x] Received from bar-queue: {message}");
        Task.Delay(1000);
        Console.WriteLine("[x] Done");
        return true;
    }
}