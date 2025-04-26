using Microsoft.Extensions.Logging;

namespace NanoRabbit;

/// <summary>
/// Message handler interface.
/// </summary>
public interface IMessageHandler
{
    /// <summary>
    /// 处理接收到的消息
    /// </summary>
    /// <param name="messageBody">消息体 (原始字节)</param>
    /// <param name="routingKey">消息的路由键 (如果适用)</param>
    /// <param name="correlationId">消息的关联 ID (如果适用)</param>
    /// <returns>如果处理成功，返回 true；否则返回 false。</returns>
    bool HandleMessage(byte[] messageBody, string? routingKey = null, string? correlationId = null);
}

public class DefaultMessageHandler : IMessageHandler
{
    private readonly ILogger<DefaultMessageHandler> _logger;
    // 可以注入其他服务，如数据库上下文、业务逻辑服务等
    // private readonly MyDbContext _dbContext;

    public DefaultMessageHandler(ILogger<DefaultMessageHandler> logger/*, MyDbContext dbContext*/)
    {
        _logger = logger;
        // _dbContext = dbContext;
    }

    public bool HandleMessage(byte[] messageBody, string? routingKey = null, string? correlationId = null)
    {
        try
        {
            // 1. 反序列化消息
            var messageString = System.Text.Encoding.UTF8.GetString(messageBody);
            _logger.LogInformation("接收到消息 (处理器): RoutingKey='{RoutingKey}', CorrelationId='{CorrelationId}', Body='{MessageBody}'",
                routingKey, correlationId, messageString);

            // TODO: 在这里根据消息内容执行你的业务逻辑
            // 例如：反序列化为具体对象、保存到数据库、调用其他服务等
            // var myMessage = JsonSerializer.Deserialize<MyMessageObject>(messageString);
            // await _dbContext.ProcessMessageAsync(myMessage);

            _logger.LogInformation("消息处理成功 (处理器)。");
            return true; // 返回 true 表示处理成功
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理消息时发生错误 (处理器)。 CorrelationId='{CorrelationId}'", correlationId);
            return false; // 返回 false 表示处理失败
        }
    }
}
