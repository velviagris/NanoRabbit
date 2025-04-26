using Microsoft.Extensions.Logging;
using NanoRabbit;

namespace Test.Publish
{
    [TestClass]
    public class PublishTest
    {
        [TestMethod]
        public void TestPublish()
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

            rabbitHelper.Publish<string>("FooProducer", "Hello from TestPublish()");
        }

        [TestMethod]
        public void TestPublishBatch()
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

            rabbitHelper.PublishBatch<string>("FooProducer", new List<string> { "Hello from TestPublishBatch() - 1", "Hello from TestPublishBatch() - 2" });
        }
        
        [TestMethod]
        public async Task TestPublishAsync()
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

            await rabbitHelper.PublishAsync<string>("FooProducer", "Hello from TestPublishAsync()");
        }

        [TestMethod]
        public async Task TestPublishBatchAsync()
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

            await rabbitHelper.PublishBatchAsync<string>("FooProducer", new List<string> { "Hello from TestPublishBatchAsync() - 1", "Hello from TestPublishBatchAsync() - 2" });
        }
    }
}