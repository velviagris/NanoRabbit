using System.Security.Authentication;

namespace NanoRabbit;


/// <summary>
/// NanoRabbit producer connect options
/// </summary>
public class ProducerOptions
{
    /// <summary>
    /// Customize producer name
    /// </summary>
    public string ProducerName { get; set; } = null!;

    /// <summary>
    /// Exchange name
    /// </summary>
    public string? ExchangeName { get; set; } = "";

    /// <summary>
    /// Publish routing-key
    /// </summary>
    public string? RoutingKey { get; set; }

    /// <summary>
    /// Exchange type, default: direct
    /// </summary>
    public string Type { get; set; } = ExchangeType.Direct;

    /// <summary>
    /// Exchange durable, default: true
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    /// Exchange auto-delete, default: false
    /// </summary>
    public bool AutoDelete { get; set; } = false;

    /// <summary>
    /// Set false to disable automatic connection recovery. Defaults to true.
    /// </summary>
    public bool AutomaticRecoveryEnabled { get; set; } = true;

    /// <summary>
    /// Set to true to enable automatic resend cached massages. Defaults to false.
    /// </summary>
    public bool AutomaticResend { get; set; } = false;

    /// <summary>
    /// Exchange additional arguments
    /// </summary>
    public IDictionary<string, object>? Arguments { get; set; }
}

/// <summary>
/// NanoRabbit consumer connect options
/// </summary>
public class ConsumerOptions
{
    /// <summary>
    /// Customize consumer name
    /// </summary>
    public string ConsumerName { get; set; } = null!;
    
    /// <summary>
    /// Customize handler name
    /// </summary>
    public string HandlerName { get; set; } = null!;

    /// <summary>
    /// Subscribe queue name
    /// </summary>
    public string QueueName { get; set; } = null!;

    /// <summary>
    /// Set false to disable automatic connection recovery. Defaults to true.
    /// </summary>
    public bool AutomaticRecoveryEnabled { get; set; } = true;

    /// <summary>
    /// BasicQos prefetchSize, default: 0
    /// </summary>
    public uint PrefetchSize { get; set; } = 0;

    /// <summary>
    /// BasicQos prefetchCount, default: 0
    /// </summary>
    public ushort PrefetchCount { get; set; } = 0;
    
    /// <summary>
    /// Set several Consumers, defaults: 1s
    /// </summary>
    public int ConsumerCount { get; set; } = 1;

    #region Additional options (declare if need)

    public bool DeclareQueue { get; set; } = false;
    public bool QueueDurable { get; set; } = true;
    public bool QueueExclusive { get; set; } = false;
    public bool QueueAutoDelete { get; set; } = false;
    public IDictionary<string, object>? QueueArguments { get; set; }
    public bool AutoAck { get; set; } = false;

    #endregion 
}

/// <summary>
/// NanoRabbit connection configurations in appsettings.json
/// </summary>
public class RabbitConfiguration
{
    /// <summary>
    /// RabbitMQ connection Uri.
    /// Example: "amqp://user:pass@hostName:port/vhost"
    /// </summary>
    public string? Uri { get; set; } = null;

    /// <summary>
    /// RabbitMQ HostName.
    /// Example: "localhost"
    /// </summary>
    public string? HostName { get; set; }
    /// <summary>
    /// RabbitMQ AmqpTcpEndpoint port.
    /// Defaults: 5672
    /// </summary>
    public int Port { get; set; } = 5672;
    /// <summary>
    /// RabbitMQ UserName.
    /// Example: "guest"
    /// </summary>
    public string? UserName { get; set; }
    /// <summary>
    /// RabbitMQ Password.
    /// Example: "guest"
    /// </summary>
    public string? Password { get; set; }
    /// <summary>
    /// RabbitMQ VirtualHost.
    /// Example: "/"
    /// </summary>
    public string? VirtualHost { get; set; }

    /// <summary>
    /// Use async-oriented consumer dispatcher. Only compatible with IAsyncBasicConsumer implementations.
    /// Defaults: false
    /// </summary>
    public bool UseAsyncConsumer { get; set; }
    
    /// <summary>
    /// Connect to RabbitMQ using TLS.
    /// </summary>
    public TLSConfig? TLSConfig { get; set; }

    /// <summary>
    /// ClientProvidedName
    /// </summary>
    public string? ConnectionName { get; set; }

    /// <summary>
    /// RabbitProducer configs.
    /// Defaults: null
    /// </summary>
    public List<ProducerOptions>? Producers { get; set; }
    /// <summary>
    /// RabbitConsumer configs.
    /// Defaults: null
    /// </summary>
    public List<ConsumerOptions>? Consumers { get; set; }
}

/// <summary>
/// TLS configs.
/// </summary>
public class TLSConfig
{
    public bool Enabled { get; set; } = true;
    public string ServerName { get; set; } = System.Net.Dns.GetHostName();
    public string CertPath { get; set; } = "/path/to/client_key.p12";
    public string CertPassphrase { get; set; } = "MySecretPassword";
    public SslProtocols Version { get; set; } = SslProtocols.Tls12;
}

/// <summary>
/// Convenience class providing compile-time names for standard exchange types.
/// </summary>
/// <remarks>
/// Use the static members of this class as values for the
/// "exchangeType" arguments for IModel methods such as
/// ExchangeDeclare. The broker may be extended with additional
/// exchange types that do not appear in this class.
/// </remarks>
public static class ExchangeType
{
    /// <summary>
    /// Exchange type used for AMQP direct exchanges.
    /// </summary>
    public const string Direct = "direct";

    /// <summary>
    /// Exchange type used for AMQP fanout exchanges.
    /// </summary>
    public const string Fanout = "fanout";

    /// <summary>
    /// Exchange type used for AMQP headers exchanges.
    /// </summary>
    public const string Headers = "headers";

    /// <summary>
    /// Exchange type used for AMQP topic exchanges.
    /// </summary>
    public const string Topic = "topic";

    private static readonly string[] s_all = { Fanout, Direct, Topic, Headers };

    /// <summary>
    /// Retrieve a collection containing all standard exchange types.
    /// </summary>
    public static ICollection<string> All()
    {
        return s_all;
    }
}
