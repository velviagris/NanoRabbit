using System.Text;
using Microsoft.Extensions.Hosting;
using NanoRabbit;
using NanoRabbit.DependencyInjection;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddRabbitHelper(rabbitConfigurationBuilder =>
{
    rabbitConfigurationBuilder.SetHostName("localhost")
        .SetPort(5672)
        .SetVirtualHost("/")
        .SetUserName("admin")
        .SetPassword("admin")
        .UseAsyncConsumer(true) // set UseAsyncConsumer to true
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
            consumer.ConsumerCount = 3;
            consumer.HandlerName = nameof(FooQueueHandler);
        })
        .AddConsumerOption(consumer =>
        {
            consumer.ConsumerName = "BarConsumer";
            consumer.QueueName = "bar-queue";
            consumer.ConsumerCount = 2;
            consumer.HandlerName = nameof(BarQueueHandler);
        });
})
.AddRabbitAsyncHandler<FooQueueHandler>()
.AddRabbitAsyncHandler<BarQueueHandler>()
.AddRabbitConsumerService();

var host = builder.Build();
await host.RunAsync();

public class FooQueueHandler : IAsyncMessageHandler
{
    public async Task<bool> HandleMessageAsync(byte[] messageBody, string? routingKey = null, string? correlationId = null)
    {
        var message = Encoding.UTF8.GetString(messageBody);
        Console.WriteLine($"[x] Received from foo-queue: {message}");
        await Task.Delay(1000);
        Console.WriteLine("[x] Done");
        return true;
    }
}

public class BarQueueHandler : IAsyncMessageHandler
{
    public async Task<bool> HandleMessageAsync(byte[] messageBody, string? routingKey = null, string? correlationId = null)
    {
        var message = Encoding.UTF8.GetString(messageBody);
        Console.WriteLine($"[x] Received from bar-queue: {message}");
        await Task.Delay(500);
        Console.WriteLine("[x] Done");
        return true;
    }
}
