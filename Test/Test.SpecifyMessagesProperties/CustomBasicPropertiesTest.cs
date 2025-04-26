using Microsoft.Extensions.Logging;
using NanoRabbit;
using RabbitMQ.Client;

namespace Test.SpecifyMessagesProperties
{
    [TestClass]
    public class CustomBasicPropertiesTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

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

            var channel = rabbitHelper.GetChannel("FooConsumer");
            IBasicProperties props = rabbitHelper.CreateBasicProperties(channel);
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;

            rabbitHelper.Publish<string>("FooProducer", "Hello from NanoRabbit", props);
        }
        
        [TestMethod]
        public void TestMethod2()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

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

            var channel = rabbitHelper.GetChannel("FooProducer");
            IBasicProperties props = rabbitHelper.CreateBasicProperties(channel);
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("latitude", 51.5252949);
            props.Headers.Add("longitude", -0.0905493);

            rabbitHelper.Publish<string>("FooProducer", "Hello from NanoRabbit", props);
        }
        
        [TestMethod]
        public void TestMethod3()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

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

            var channel = rabbitHelper.GetChannel("FooProducer");
            IBasicProperties props = rabbitHelper.CreateBasicProperties(channel);
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Expiration = "36000000";

            rabbitHelper.Publish<string>("FooProducer", "Hello from NanoRabbit", props);
        }
    }
}