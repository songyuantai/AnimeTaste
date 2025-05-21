using AnimeTaste.Service.Cache;
using AnimeTaste.WebApi.Auth;
using AnimeTaste.WebApi.Storage;
using JikanDotNet;
using StackExchange.Redis;

namespace AnimeTaste.WebApi.Extensions
{
    public static class DiExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddSingleton<JwtHelper>();

            services.AddTransient<MinioService>();

            services.AddSingleton<IJikan, Jikan>();

            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration config)
        {
            var connectString = config[RedisOption.KEY + ":ConnectString"] ?? "";
            if (string.IsNullOrEmpty(connectString))
            {
                throw new Exception("redis配置为空，请检查！");
            }

            services.Configure<RedisOption>(config.GetSection(RedisOption.KEY));
            var redis = ConnectionMultiplexer.Connect(connectString);
            services.AddSingleton(redis);
            return services;
        }

    }
}
