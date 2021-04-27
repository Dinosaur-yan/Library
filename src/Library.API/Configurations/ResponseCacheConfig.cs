using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Library.API.Configurations
{
    public static class ResponseCacheConfig
    {
        public static void AddResponseCachingConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddResponseCaching(options =>
            {
                // 是否区分请求路径的大小写，默认为true
                options.UseCaseSensitivePaths = true;
                // 允许缓存响应正文的最大值，默认为64MB
                options.MaximumBodySize = 1024;
                // 设置缓存响应中间件的缓存大小，默认为100MB
                // options.SizeLimit = 100 * 1024;
            });
        }

        public static void UseResponseCachingSetup(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseResponseCaching();
        }

        /**
         * 缓存是一种通过存储资源的备份，在请求时返回资源备份的技术
         * 1. http缓存（是否使用缓存Cache-Control、缓存有效时间Expires，只有当请求是GET或HEAD并且返回结果为200 OK状态码时，响应才支持缓存）
         *  常见的Cache-Control指令：
         *  public: 表明响应可以被任何对象（如发送请求的客户端和代理服务器）缓存
         *  private: 表明响应只能为单个用户缓存，不能作为共享缓存（及代理服务器不能缓存它）
         *  max-age: 设置缓存的最大存储时间，超过这个时间缓存被认为过期，单位为秒；当与Expires同时出现时，优先使用max-age
         *  no-cache: 必须到原始服务器验证后才能使用的缓存
         *      验证缓存自愿的方式有两种
         *      (1) 通过响应消息头中的Last-Modified（资源最后更新的时间），在验证时，需要在请求头中添加If-Modified-Since消息头，这个值是客户端最后一次收到该响应资源Last-Modified的值
         *      (2) 通过使用实体标签ETag（Entity Tag），在验证时，需要在请求头中添加If-None-Match，这个值是客户端最后一次从服务器获取的ETag值；
         *          当ETag一致时返回304Not Modified状态码，不一致时处理请求，
         *  no-store: 缓存不应存储有关客户端请求或服务器的任何内容 
         */
        public static void AddCacheProfiles(this IDictionary<string, CacheProfile> cacheProfiles)
        {
            /**
             * 通过使用[ResponseCache(CacheProfileName = "Default")]在指定接口前使用该配置
             * 或者使用[ResponseCache(Duration = 60)]
             */
            cacheProfiles.Add("Default", new CacheProfile
            {
                // max-age=60
                Duration = 60,
                // 根据不同的查询关键字区分不同的缓存
                // VaryByQueryKeys = new string[] { "sortBy", "searchQuery" }
            });

            cacheProfiles.Add("Never", new CacheProfile
            {
                /**
                 * Location属性可以改变缓存的位置
                 *      Any: 设置Cache-Control的值为public
                 *      Client: 设置Cache-Control的值为private
                 *      None: 设置Cache-Control的值为nocache
                 */
                Location = ResponseCacheLocation.None,
                NoStore = true
            });
        }
    }
}
