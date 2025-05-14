using Microsoft.Extensions.DependencyInjection;

namespace NanoRabbit.DependencyInjection
{
    /// <summary>
    /// Rabbit Handler Extensions
    /// </summary>
    public static class RabbitHandlerExtensions
    {
        /// <summary>
        /// Add Keyed Scoped Rabbit Handler
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddRabbitHandler<THandler>(this IServiceCollection services)
            where THandler : class, IMessageHandler
        {
            var serviceKey = typeof(THandler).Name;
            services.AddKeyedScoped<IMessageHandler, THandler>(serviceKey);
            return services;
        }
        
        /// <summary>
        /// Add Keyed Scoped Asynchronous Rabbit Handler
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TAsyncHandler"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddRabbitAsyncHandler<TAsyncHandler>(this IServiceCollection services)
            where TAsyncHandler : class, IAsyncMessageHandler
        {
            var serviceKey = typeof(TAsyncHandler).Name;
            services.AddKeyedScoped<IAsyncMessageHandler, TAsyncHandler>(serviceKey);
            return services;
        }
    }
}