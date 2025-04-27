using System.Text;
using Example.MultiConnections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NanoRabbit;
using NanoRabbit.DependencyInjection;

var builder = Host.CreateApplicationBuilder();


builder.Services.AddKeyedRabbitHelper("DefaultRabbitHelper", rabbitConfigurationBuilder =>
{
    rabbitConfigurationBuilder.SetHostName("localhost")
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
            consumer.HandlerName = nameof(FooQueueHandler);
        });
});

builder.Services.AddKeyedRabbitHelper("TestRabbitHelper", rabbitConfigurationBuilder =>
{
    rabbitConfigurationBuilder.SetHostName("localhost")
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
            consumer.HandlerName = nameof(BarQueueHandler);
        });
});

builder.Services.AddRabbitHandler<FooQueueHandler>();
builder.Services.AddRabbitHandler<BarQueueHandler>();

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