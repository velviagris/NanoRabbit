![NanoRabbit logo](https://raw.githubusercontent.com/cgcel/NanoRabbit/master/Img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/NanoRabbit.svg)](https://nuget.org/packages/NanoRabbit) [![Nuget Downloads](https://img.shields.io/nuget/dt/NanoRabbit)](https://www.nuget.org/packages/NanoRabbit) [![License](https://img.shields.io/github/license/cgcel/NanoRabbit)](https://github.com/cgcel/NanoRabbit)
[![codebeat badge](https://codebeat.co/badges/a37a04d9-dd8e-4177-9b4c-c17526910f7e)](https://codebeat.co/projects/github-com-cgcel-nanorabbit-master)

## About

NanoRabbit, A **Lightweight** RabbitMQ .NET 3rd party library for .NET 8 and up, which makes a simple way to manage
**Multiple** *connections*, *producers*, *consumers*, and easy to use.

## Building

| Branch |                                                                                 Building Status                                                                                 |
|:------:|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| master | [![build](https://github.com/cgcel/NanoRabbit/actions/workflows/build.yml/badge.svg?branch=master&event=push)](https://github.com/cgcel/NanoRabbit/actions/workflows/build.yml) |
|  dev   |  [![build](https://github.com/cgcel/NanoRabbit/actions/workflows/build.yml/badge.svg?branch=dev&event=push)](https://github.com/cgcel/NanoRabbit/actions/workflows/build.yml)   |

## Features

- Customize the name of producers, consumers.
- Dependency injection available.
- Multiple connections, producers, and consumers can be created.

## Installation

You can get NanoRabbit by grabbing the latest [NuGet](https://www.nuget.org/packages/NanoRabbit) package.

See [Wiki](https://github.com/cgcel/NanoRabbit/wiki/Installation) for more details.

## Version

|  NanoRabbit   | RabbitMQ.Client |     .NET      |
|:-------------:|:---------------:|:-------------:|
| 0.0.1 ~ 0.1.8 |    obsolete     |   obsolete    |
| 0.1.9 ~ 0.2.3 |   6.5.0-6.8.1   | 6.0, 7.0, 8.0 |
|     0.2.4     |      6.5.0-6.8.1      |      8.0      |

## Document

For details, see: [NanoRabbit Wiki](https://github.com/cgcel/NanoRabbit/wiki).

## QuickStart

> *NanoRabbit is designed as a library depends on **NAMING** Connections, Producers and Consumers. So it's important to
set
a **UNIQUE NAME** for each Connections, Producers and Consumers.*

For more, please visit the [Examples](https://github.com/cgcel/NanoRabbit/tree/master/Example).

### Setup RabbitProducers && RabbitConsumers

#### RabbitProducer

Register a RabbitMQ Producer by calling
`RabbitHelper(RabbitConfiguration rabbitConfig, ILogger<RabbitHelper>? logger = null)`, and configure it.

```csharp
using NanoRabbit;
using NanoRabbit.Connection;

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
    Producers = new List<ProducerOptions> { new ProducerOptions {
            ProducerName = "FooProducer",
            ExchangeName = "amq.topic",
            RoutingKey = "foo.key",
            Type = ExchangeType.Topic
        } 
    }
}, logger);
```

#### RabbitConsumer

Register a RabbitMQ Consumer by calling `RabbitHelper()`, and configure it.

```csharp
using NanoRabbit;
using NanoRabbit.Connection;

var rabbitHelper = new RabbitHelper(rabbitConfig: new RabbitConfiguration
{
    HostName = "localhost",
    Port = 5672,
    VirtualHost = "/",
    UserName = "admin",
    Password = "admin",
    Consumers = new List<ConsumerOptions> { new ConsumerOptions {
            ConsumerName= "FooConsumer",
            QueueName = "foo-queue"
        }
    }
}, logger);
```

### Simple Publish Messages

[After](#rabbitproducer) registering a `RabbitProducer` in the `RabbitHelper`, you can simply publish a message by
calling `Publish<T>(string producerName, T message)`.

```csharp
rabbitHelper.Publish<string>("FooProducer", "Hello from NanoRabbit");
```

Asynchronously publishing is also available:

```csharp
await rabbitHelper.PublishAsync<string>("FooProducer", "Hello from NanoRabbit");
```

### Simple Consume Messages

[After](#rabbitconsumer) registering a `RabbitConsumer` in the `RabbitConsumer`, you can simply consume a message by
calling `AddConsumer(string consumerName, Action<string> onMessageReceived, int consumers = 1)`.

```csharp
while (true)
{
    rabbitHelper.AddConsumer("FooConsumer", message =>
    {
        Console.WriteLine(message);
    });
}
```

### Forward messages

> Working on it.

### DependencyInjection

NanoRabbit provides some functions to inject IRabbitHelper in a simple way.

#### AddRabbitProducer

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Configure the RabbitMQ Connection
builder.Services.AddRabbitHelper(builder =>
{
    builder.SetHostName("localhost")
        .SetPort(5672)
        .SetVirtualHost("/")
        .SetUserName("admin")
        .SetPassword("admin")
        .AddProducerOption(producer =>
        {
            producer.ProducerName = "FooProducer";
            producer.ExchangeName = "amq.topic";
            producer.RoutingKey = "foo.key";
            producer.Type = ExchangeType.Topic;
        });
});

using IHost host = builder.Build();

host.Run();
```

#### AddRabbitConsumer

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Configure the RabbitMQ Connection
builder.Services.AddRabbitHelper(builder =>
{
    builder.SetHostName("localhost")
        .SetPort(5672)
        .SetVirtualHost("/")
        .SetUserName("admin")
        .SetPassword("admin")
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
.AddRabbitHandler<FooQueueHandler>()
.AddRabbitHandler<BarQueueHandler>()
.AddRabbitConsumerService();

using IHost host = builder.Build();

host.Run();
```

More *Dependency Injection* Usage at [Wiki](https://github.com/cgcel/NanoRabbit/wiki/DependencyInjection).

## Contributing

1. Fork this repository.
2. Create a new branch in you current repos from the **dev** branch.
3. Push commits and create a Pull Request (PR) to NanoRabbit.

## Todo

- [x] Basic Consume & Publish
- [x] DependencyInjection
- [x] Logging
- [x] Support .NET latest frameworks
- [ ] Forward messages
- [x] ASP.NET support
- [x] TLS support
- [x] Re-trying of failed sends
- [x] Basic [RabbitMQ.Client](https://github.com/rabbitmq/rabbitmq-dotnet-client) functions extensions

## Thanks

- Visual Studio 2022
- [RabbitMQ.Client](https://github.com/rabbitmq/rabbitmq-dotnet-client)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [Masstransit](https://github.com/masstransit/masstransit)
- [EasyNetQ](https://github.com/autofac/Autofac)
- [Polly.Core](https://github.com/App-vNext/Polly)

## License

NanoRabbit is licensed under the [MIT](https://github.com/cgcel/NanoRabbit/blob/dev/LICENSE.txt) license.
