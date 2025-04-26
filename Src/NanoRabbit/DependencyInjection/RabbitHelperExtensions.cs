using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoRabbit.Helper;

namespace NanoRabbit.DependencyInjection;

/// <summary>
/// RabbitHelper extensions.
/// </summary>
public static class RabbitHelperExtensions
{
    private static readonly ILogger NullLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    /// <summary>
    /// Add a singleton service of the type specified in IRabbitHelper with a factory specified in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitHelper(this IServiceCollection services,
        Action<RabbitConfigurationBuilder> builder, Func<IServiceCollection, ILogger>? loggerFactory = null)
    {
        var rabbitConfigBuilder = new RabbitConfigurationBuilder();
        builder.Invoke(rabbitConfigBuilder);
        var rabbitConfig = rabbitConfigBuilder.Build();

        services.AddSingleton(rabbitConfig);
        services.AddSingleton<IRabbitHelper>(_ => new RabbitHelper(rabbitConfig, GetLogger(services, loggerFactory)));
        return services;
    }

    /// <summary>
    /// Add a keyed singleton service of the type specified in IRabbitHelper with a factory specified in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="key">An object that specifies the key of service object to get.</param>
    /// <param name="builders"></param>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static IServiceCollection AddKeyedRabbitHelper(this IServiceCollection services, object? key,
        Action<RabbitConfigurationBuilder> builders, Func<IServiceCollection, ILogger>? loggerFactory = null)
    {
        var rabbitConfigBuilder = new RabbitConfigurationBuilder();
        builders.Invoke(rabbitConfigBuilder);
        var rabbitConfig = rabbitConfigBuilder.Build();

        services.AddKeyedSingleton(key, rabbitConfig);
        services.AddKeyedSingleton<IRabbitHelper>(key, (_, _) =>
        {
            var rabbitHelper = new RabbitHelper(rabbitConfig, GetLogger(services, loggerFactory));
            return rabbitHelper;
        });
        return services;
    }

    /// <summary>
    /// Add a singleton service of the type specified in IRabbitHelper by reading configurations of appsettings.json.
    /// </summary>
    /// <typeparam name="TRabbitConfiguration"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static IServiceCollection AddRabbitHelperFromAppSettings<TRabbitConfiguration>(
        this IServiceCollection services, IConfiguration configuration,
        Func<IServiceCollection, ILogger>? loggerFactory = null)
        where TRabbitConfiguration : RabbitConfiguration, new()
    {
        TRabbitConfiguration? rabbitConfig = configuration.ReadSettings<TRabbitConfiguration>();

        if (rabbitConfig != null)
        {
            services.AddSingleton(rabbitConfig);
            services.AddSingleton<IRabbitHelper>(
                _ => new RabbitHelper(rabbitConfig, GetLogger(services, loggerFactory)));
        }
        else
        {
            throw new Exception("NanoRabbit Configuration is incorrect.");
        }

        return services;
    }

    /// <summary>
    /// Add a keyed singleton service of the type specified in IRabbitHelper by reading configurations of appsettings.json.
    /// </summary>
    /// <typeparam name="TRabbitConfiguration"></typeparam>
    /// <param name="services"></param>
    /// <param name="key"></param>
    /// <param name="configuration"></param>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static IServiceCollection AddKeyedRabbitHelperFromAppSettings<TRabbitConfiguration>(
        this IServiceCollection services, object? key, IConfiguration configuration,
        Func<IServiceCollection, ILogger>? loggerFactory = null)
        where TRabbitConfiguration : RabbitConfiguration, new()
    {
        TRabbitConfiguration? rabbitConfig = configuration.ReadSettings<TRabbitConfiguration>();

        if (rabbitConfig != null)
        {
            services.AddKeyedSingleton(key, rabbitConfig);
            services.AddKeyedSingleton<IRabbitHelper>(key, (_, _) =>
            {
                var rabbitHelper = new RabbitHelper(rabbitConfig, GetLogger(services, loggerFactory));
                return rabbitHelper;
            });
        }
        else
        {
            throw new Exception("NanoRabbit Configuration is incorrect.");
        }
        return services;
    }

    /// <summary>
    /// Get specific IRabbitHelper service by key.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static IRabbitHelper GetRabbitHelper(this IServiceProvider serviceProvider, object? key)
    {
        return serviceProvider.GetRequiredKeyedService<IRabbitHelper>(key);
    }

    #region Private

    private static ILogger GetLogger(IServiceCollection services,
        Func<IServiceCollection, ILogger>? loggerFactory = null)
    {
        var logger = loggerFactory is null ? NullLogger : loggerFactory(services);

        return logger;
    }

    #endregion
}