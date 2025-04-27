using Example.ProducerInConsumer;
using Microsoft.Extensions.Hosting;
using NanoRabbit;
using NanoRabbit.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

// Configure the RabbitMQ Connection
builder.Services.AddRabbitHelper(rabbitConfigurationBuilder =>
{
    rabbitConfigurationBuilder.SetHostName("localhost")
        .SetPort(5672)
        .SetVirtualHost("/")
        .SetUserName("admin")
        .SetPassword("admin")
        .AddProducerOption(producer =>
        {
            producer.ProducerName = "BarProducer";
            producer.ExchangeName = "amq.direct";
            producer.RoutingKey = "bar.key";
            producer.Type = ExchangeType.Direct;
        })
        .AddConsumerOption(consumer =>
        {
            consumer.ConsumerName = "FooConsumer";
            consumer.QueueName = "foo-queue";
            consumer.ConsumerCount = 3;
        });
})
.AddRabbitHandler<FooQueueHandler>().AddRabbitConsumerService();

using IHost host = builder.Build();

await host.RunAsync();