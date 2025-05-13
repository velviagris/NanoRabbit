using RabbitMQ.Client;
using System.Threading.Channels;

namespace NanoRabbit;

/// <summary>
/// RabbitHelper interface
/// </summary>
public interface IRabbitHelper
{
    #region basic functions

    /// <summary>
    /// Publish message, extended from BasicPublish().
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="producerName"></param>
    /// <param name="message"></param>
    /// <param name="properties"></param>
    public void Publish<T>(string producerName, T message, IBasicProperties? properties = null);
    
    /// <summary>
    /// Publish a batch of messages, extended from BasicPublish().
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="producerName"></param>
    /// <param name="messageList"></param>
    /// <param name="properties"></param>
    public void PublishBatch<T>(string producerName, IEnumerable<T?> messageList, IBasicProperties? properties = null);
    
    /// <summary>
    /// Publish message asynchronously, extended from BasicPublish().
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="producerName"></param>
    /// <param name="message"></param>
    /// <param name="properties"></param>
    public Task PublishAsync<T>(string producerName, T message, IBasicProperties? properties = null);
    
    /// <summary>
    /// Publish a batch of messages asynchronously, extended from BasicPublish().
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="producerName"></param>
    /// <param name="messageList"></param>
    /// <param name="properties"></param>
    public Task PublishBatchAsync<T>(string producerName, IEnumerable<T?> messageList, IBasicProperties? properties = null);
    
    /// <summary>
    /// Add a consumer by using predefined consumer configs.
    /// </summary>
    /// <param name="consumerName"></param>
    /// <param name="onMessageReceived"></param>
    /// <param name="consumers"></param>
    public void AddConsumer(string consumerName, Action<string> onMessageReceived, int consumers = 1);
    
    /// <summary>
    /// Add an asynchronous consumer by using predefined consumer configs.
    /// </summary>
    /// <param name="consumerName"></param>
    /// <param name="onMessageReceivedAsync"></param>
    /// <param name="consumers"></param>
    public void AddAsyncConsumer(string consumerName, Func<string, Task> onMessageReceivedAsync, int consumers = 1);

    #endregion

    #region utils

    /// <summary>
    /// Get channel.
    /// </summary>
    /// <param name="channelName"></param>
    /// <returns></returns>
    public IModel GetChannel(string channelName);

    /// <summary>
    /// Release channel.
    /// </summary>
    /// <param name="channelName"></param>
    public void ReleaseChannel(string channelName);
    
    /// <summary>
    /// Declare an exchange.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="exchangeName"></param>
    /// <param name="exchangeType"></param>
    /// <param name="durable"></param>
    /// <param name="autoDelete"></param>
    /// <param name="arguments"></param>
    public void ExchangeDeclare(IModel channel, string exchangeName, string exchangeType, bool durable = false, bool autoDelete = false, IDictionary<string, object>? arguments = null);

    /// <summary>
    /// Bind an exchange to an exchange.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    /// <param name="routingKey"></param>
    /// <param name="arguments"></param>
    public void ExchangeBind(IModel channel, string destination, string source, string routingKey, IDictionary<string, object> arguments);

    /// <summary>
    /// Delete an exchange.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="exchangeName"></param>
    /// <param name="ifUnused"></param>
    public void ExchangeDelete(IModel channel, string exchangeName, bool ifUnused);

    /// <summary>
    /// Declare a queue.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="queueName"></param>
    /// <param name="durable"></param>
    /// <param name="exclusive"></param>
    /// <param name="autoDelete"></param>
    /// <param name="arguments"></param>
    public void QueueDeclare(IModel channel, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object>? arguments = null);

    /// <summary>
    /// Bind a queue to an exchange.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="queueName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="routingKey"></param>
    /// <param name="arguments"></param>
    public void QueueBind(IModel channel, string queueName, string exchangeName, string routingKey, IDictionary<string, object>? arguments = null);

    /// <summary>
    /// Delete a queue.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="queueName"></param>
    /// <param name="ifUnused"></param>
    /// <param name="ifEmpty"></param>
    public void QueueDelete(IModel channel, string queueName, bool ifUnused, bool ifEmpty);

    /// <summary>
    /// Purge a queue of messages.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="queueName"></param>
    public void QueuePurge(IModel channel, string queueName);
    /// <summary>
    /// Create a custom BasicProperties.
    /// </summary>
    /// <param name="channel"></param>
    public IBasicProperties CreateBasicProperties(IModel channel);

    #endregion
}