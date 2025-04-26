using Microsoft.Extensions.DependencyInjection;

namespace NanoRabbit.DependencyInjection
{
    public static class RabbitHandlerExtensions
    {
        public static IServiceCollection AddRabbitHandler<THandler>(this IServiceCollection services)
            where THandler : class, IMessageHandler
        {
            services.AddScoped<IMessageHandler, THandler>();
            return services;
        }
        
        public static IServiceCollection AddRabbitAsyncHandler<TAsyncHandler>(this IServiceCollection services)
            where TAsyncHandler : class, IAsyncMessageHandler
        {
            services.AddScoped<IAsyncMessageHandler, TAsyncHandler>();
            return services;
        }
        
        public static IServiceCollection AddKeyedRabbitHandler<THandler>(this IServiceCollection services, object? key)
            where THandler : class, IMessageHandler
        {
            var serviceKey = key ?? typeof(THandler).Name;
            services.AddKeyedScoped<IMessageHandler, THandler>(serviceKey);
            return services;
        }
        
        public static IServiceCollection AddKeyedRabbitAsyncHandler<TAsyncHandler>(this IServiceCollection services, object? key)
            where TAsyncHandler : class, IAsyncMessageHandler
        {
            var serviceKey = key ?? typeof(TAsyncHandler).Name;
            services.AddKeyedScoped<IAsyncMessageHandler, TAsyncHandler>(serviceKey);
            return services;
        }
    }
}