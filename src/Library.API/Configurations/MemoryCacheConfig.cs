using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Configurations
{
    public static class MemoryCacheConfig
    {
        /**
         * 内存缓存事实上是一个键值对字典
         */
        public static void AddMemoryCacheConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddMemoryCache();
        }
    }
}
