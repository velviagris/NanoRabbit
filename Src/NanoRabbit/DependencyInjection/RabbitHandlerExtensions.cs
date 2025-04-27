using Microsoft.Extensions.DependencyInjection;

namespace NanoRabbit.DependencyInjection
{
    public static class RabbitHandlerExtensions
    {
        public static IServiceCollection AddRabbitHandler<THandler>(this IServiceCollection services)
            where THandler : class, IMessageHandler
        {
            var serviceKey = typeof(THandler).Name;
            services.AddKeyedScoped<IMessageHandler, THandler>(serviceKey);
            return services;
        }
        
        public static IServiceCollection AddRabbitAsyncHandler<TAsyncHandler>(this IServiceCollection services)
            where TAsyncHandler : class, IAsyncMessageHandler
        {
            var serviceKey = typeof(TAsyncHandler).Name;
            services.AddKeyedScoped<IAsyncMessageHandler, TAsyncHandler>(serviceKey);
            return services;
        }
    }
}