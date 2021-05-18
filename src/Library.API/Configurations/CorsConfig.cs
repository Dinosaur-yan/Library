using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Configurations
{
    public static class CorsConfig
    {
        /**
         * CORS，全称Cross-Origin Resource Sharing（跨域资源共享），是一种允许当前域的资源能被其他域访问的机制。
         * https://(协议) 127.0.0.1(主机) :5001(端口) /api/values，3项中有1项不同，那么该资源就会被认为来自不同的域，则浏览器不允许访问
         * 
         * 对于跨域资源访问，CORS会将它们分为两种类型：简单请求和非简单请求。
         * 简单请求：
         *      1. 请求方法为GET、HEAD、POST三者之一
         *      2. 如果请求方法为POST，则Content-Type消息头的值只允许为这3项：application/x- www-form-urlencoded、multipart/form-data、text/plain
         *      3. 不包含自定义消息头
         * 
         * 非简单请求：不满足其中任何一个条件，如请求方法为PUT和DELETE等，则该请求为非简单请求
         *             非简单请求要比简单请求略为复杂一些。所谓非简单请求，是指在向服务器发送实际请求之前，先发送一个OPTIONS方法的请求，以确认发送正式请求是否安全
         * 
         */
        public static void AddCorsConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // CORS中间件应添加在任何可能会用到CORS功能的中间件之前
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllMethodsPolicy", builder => builder.WithOrigins("https://localhost:6001").AllowAnyMethod());
                options.AddPolicy("AllowAnyOriginsPolicy", builder => builder.AllowAnyOrigin());
                options.AddDefaultPolicy(builder => builder.WithOrigins("https://localhost:6001"));
            });
        }

        public static void UseCorsSetup(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            // CORS中间件应添加在任何可能会用到CORS功能的中间件之前
            // WithOrigins方法添加了允许CORS访问的源，该方法接受一个或多个字符串形式的URL。需要注意的是，所指定的URL末尾不能包含/，否则会导致匹配失败

            // 方法一：
            app.UseCors(builder => builder.WithOrigins("https://localhost:6001"));

            // 方法二：
            app.UseCors("AllowAllMethodsPolicy");

            // 使用CORS中间件能够为整个应用程序添加CORS功能，如果仅希望为MVC应用程序中的某个Controller或某个Action添加CORS，
            // 那么就需要使用[EnableCors]特性，此时应将CORS中间件从请求管道中移除,[DisableCors]禁用Cors支持
            //    [EnableCors]
            //    public class DemoController : Controller
            //    {
            //         可以通过在操作级别应用其他角色授权属性来进一步限制访问权限，只有Administrator可以访问
            //         [EnableCors("AllowAllMethodsPolicy")]
            //         public ActionResult ShutDown()
            //         {
            //         }
            //    }
        }
    }
}
