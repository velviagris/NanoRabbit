using System.Text;
using Example.ReadSettings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NanoRabbit;
using NanoRabbit.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddRabbitHelperFromAppSettings<FooConfiguration>(builder.Configuration)
    .AddRabbitHandler<FooQueueHandler>()
    .AddRabbitHandler<BarQueueHandler>().AddRabbitConsumerServiceFromAppSettings<FooConfiguration>(builder.Configuration);

builder.Services.AddHostedService<PublishService>();

var host = builder.Build();
await host.RunAsync();

public class FooQueueHandler : IMessageHandler
{
    public bool HandleMessage(byte[] messageBody, string? routingKey = null, string? correlationId = null)
    {
        var message = Encoding.UTF8.GetString(messageBody);
        Console.WriteLine($"[x] Received from foo-queue: {message}");
        Task.Delay(1000).Wait();
        Console.WriteLine("[x] Done");
        return true;
    }
}

public class BarQueueHandler : IMessageHandler
{
    public bool HandleMessage(byte[] messageBody, string? routingKey = null, string? correlationId = null)
    {
        var message = Encoding.UTF8.GetString(messageBody);
        Console.WriteLine($"[x] Received from bar-queue: {message}");
        Task.Delay(500).Wait();
        Console.WriteLine("[x] Done");
        
        return true;
    }
}

public class FooConfiguration : RabbitConfiguration { }