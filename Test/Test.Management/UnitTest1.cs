using Microsoft.Extensions.Logging;
using NanoRabbit;

namespace Test.Management
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestExchangeDeclare()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            var rabbitHelper = new RabbitHelper(rabbitConfig: new RabbitConfiguration
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin",
                Port = 5672,
                VirtualHost = "/",
                Producers = new List<ProducerOptions>
                {
                    new ProducerOptions
                    {
                        ProducerName = "FooProducer",
                        ExchangeName = "amq.topic",
                        RoutingKey = "foo.key",
                        Type = ExchangeType.Topic
                    }
                }
            }, logger);

            var channel = rabbitHelper.GetChannel("FooProducer");
            rabbitHelper.ExchangeDeclare(channel, "test.topic", ExchangeType.Topic);
        }
        
        [TestMethod]
        public void TestQueueDeclare()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            var rabbitHelper = new RabbitHelper(rabbitConfig: new RabbitConfiguration
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin",
                Port = 5672,
                VirtualHost = "/",
                Producers = new List<ProducerOptions>
                {
                    new ProducerOptions
                    {
                        ProducerName = "FooProducer",
                        ExchangeName = "amq.topic",
                        RoutingKey = "foo.key",
                        Type = ExchangeType.Topic
                    }
                }
            }, logger);

            var channel = rabbitHelper.GetChannel("FooProducer");
            rabbitHelper.QueueDeclare(channel, "test-queue");
        }
        
        [TestMethod]
        public void TestQueueBind()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            var rabbitHelper = new RabbitHelper(rabbitConfig: new RabbitConfiguration
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin",
                Port = 5672,
                VirtualHost = "/",
                Producers = new List<ProducerOptions>
                {
                    new ProducerOptions
                    {
                        ProducerName = "FooProducer",
                        ExchangeName = "amq.topic",
                        RoutingKey = "foo.key",
                        Type = ExchangeType.Topic
                    }
                }
            }, logger);

            var channel = rabbitHelper.GetChannel("FooProducer");
            rabbitHelper.QueueBind(channel, "test-queue", "test.topic", "test.key", null);
        }
        
        [TestMethod]
        public void TestQueueDelete()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            var rabbitHelper = new RabbitHelper(rabbitConfig: new RabbitConfiguration
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin",
                Port = 5672,
                VirtualHost = "/",
                Producers = new List<ProducerOptions>
                {
                    new ProducerOptions
                    {
                        ProducerName = "FooProducer",
                        ExchangeName = "amq.topic",
                        RoutingKey = "foo.key",
                        Type = ExchangeType.Topic
                    }
                }
            }, logger);

            var channel = rabbitHelper.GetChannel("FooProducer");
            rabbitHelper.QueueDelete(channel, "test-queue", false, false);
        }
        
        [TestMethod]
        public void TestQueuePurge()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            var rabbitHelper = new RabbitHelper(rabbitConfig: new RabbitConfiguration
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin",
                Port = 5672,
                VirtualHost = "/",
                Producers = new List<ProducerOptions>
                {
                    new ProducerOptions
                    {
                        ProducerName = "FooProducer",
                        ExchangeName = "amq.topic",
                        RoutingKey = "foo.key",
                        Type = ExchangeType.Topic
                    }
                }
            }, logger);

            var channel = rabbitHelper.GetChannel("FooProducer");
            rabbitHelper.QueuePurge(channel, "test-queue");
        }
    }
}