using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Configurations
{
    public static class RedisCacheConfig
    {
        public static void AddRedisCacheConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddStackExchangeRedisCache(options =>
            {
                // 连接到 Redis 的配置
                options.Configuration = configuration["Caching:Host"];
                // Redis 实例名称
                options.InstanceName = configuration["Caching:Instanc"];
            });
        }
    }
}
