using System.Text;
using NanoRabbit;

namespace Example.ProducerInConsumer;

public class FooQueueHandler : IMessageHandler
{
    private readonly IRabbitHelper _rabbitHelper;

    public FooQueueHandler(IRabbitHelper rabbitHelper)
    {
        _rabbitHelper = rabbitHelper;
    }

    public bool HandleMessage(byte[] messageBody, string? routingKey = null, string? correlationId = null)
    {
        var message = Encoding.UTF8.GetString(messageBody);
        Console.WriteLine($"[x] Received from foo-queue: {message}");

        _rabbitHelper.Publish("BarProducer", $"forwared from foo-queue: {message}");

        Console.WriteLine("Forwarded a message from foo-queue");

        Console.WriteLine("[x] Done");
        
        return true;
    }
}