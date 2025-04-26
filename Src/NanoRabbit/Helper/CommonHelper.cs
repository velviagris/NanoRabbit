using Microsoft.Extensions.Configuration;

namespace NanoRabbit.Helper
{
    public static class CommonHelper
    {
        /// <summary>
        /// Read NanoRabbit configs in appsettings.json
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static TRabbitConfiguration? ReadSettings<TRabbitConfiguration>(this IConfiguration configuration)
        {
            var configSection = configuration.GetSection(typeof(TRabbitConfiguration).Name);
            if (!configSection.Exists())
            {
                throw new Exception($"Configuration section '{typeof(TRabbitConfiguration).Name}' not found.");
            }

            var rabbitConfig = configSection.Get<TRabbitConfiguration>();
            return rabbitConfig;
        }
    }
}