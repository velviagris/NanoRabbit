using Microsoft.Extensions.Logging;
using NanoRabbit;

namespace Test.Logger
{
    [TestClass]
    public class NormalLoggerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
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
                Producers = new List<ProducerOptions> { 
                    new ProducerOptions {
                        ProducerName = "FooProducer",
                        ExchangeName = "amq.topic",
                        RoutingKey = "foo.key"
                    }
                },
                Consumers = new List<ConsumerOptions> { 
                    new ConsumerOptions {
                        ConsumerName= "FooConsumer",
                        QueueName = "foo-queue"
                    }
                }
            }, logger);

            rabbitHelper.Publish<string>("FooProducer", "Hello from NanoRabbit");
        }
    }
}