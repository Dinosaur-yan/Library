using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Configurations
{
    public static class VersioningConfig
    {
        public static void AddApiVersioningConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddApiVersioning(options =>
            {
                // 当客户端未提供版本时是否实用默认版本，默认值为false
                // true >> 客户端访问api时不需要显式的提供版本，方便那些原来并没有提供版本功能的api应用程序添加这一功能能，此时客户端仍可通过原来的方式访问api
                // false >> 在请求不包含版本时则会返回400 Bad Rqeuest状态码
                options.AssumeDefaultVersionWhenUnspecified = true;
                // 指明了默认版本
                options.DefaultApiVersion = new ApiVersion(1, 0);
                // 指明是否在http响应消息头中包含api-supported-versions和api-deprecated-versions
                // api-supported-versions: 当前api支持的所有版本列表
                // api-deprecated-versions: 当前api将不再使用的版本列表
                options.ReportApiVersions = true;
                // 获取指定版本内容
                // 方法一：使用查询字符串https://localhost:5001/api/values/1?api-version=2.0，
                //         默认字符串为api-version，可通过ApiVersionReader属性修改为ver，https://localhost:5001/api/values/1?ver=2.0，
                // 方法二：通过路径形式来访问指定版本的api，[Route("api/v{version:apiVersion}/values")]
                // 方法三：在消息头中添加api-version项，它的值为要访问的版本，缺点：浏览器不支持自定义消息头;(默认不支持，需修改HeaderApiVersionReader属性)
                // 方法四：通过媒体类型获取，消息中包括Content-Type和Accept时，优先使用Content-Type，默认属性为v，例：Accept：*/*;v=2，修改为version后，Accept：*/*;version=2
                // 以上四种方式推荐使用前两种方式，第四种用媒体类型的方式在RESTful API中比较适用

                // 使用查询字符串
                // options.ApiVersionReader = new QueryStringApiVersionReader("ver");

                // 使用消息头
                // options.ApiVersionReader = new HeaderApiVersionReader("api-version");

                // 通过媒体类型获取
                // options.ApiVersionReader = new MediaTypeApiVersionReader();

                // 同时支持多种方式
                // 当支持多种方式时，则所有方式指定的版本信息必须一致，否则服务端会提示版本信息不明确的错误消息
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("ver"),
                    new HeaderApiVersionReader("api-version"),
                    new MediaTypeApiVersionReader("version"));
            });
        }
    }
}
