using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Configurations
{
    public static class HttpsConfig
    {
        public static void AddHttpsConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHttpsRedirection(option =>
            {
                // RedirectStatusCode用于设置重定向时的状态码，它的默认值为307
                option.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                // HttpsPort属性则表示重定向URL中要用到的端口号
                option.HttpsPort = 5001;
            });

            // HSTS全称HTTP Strict-Transport-Security，即HTTP严格安全传输
            // 作为一个Web安全策略机制，HSTS的作用是强制客户端使用HTTPS与服务器建立连接
            // HSTS的实现方式是在响应消息中添加Strict-Transport-Security消息头，该消息头可以使浏览器在接下来指定的时间内，强制当前域名只能通过HTTPS进行访问
            // Strict-Transport-Security: <max-age=>[; IncludeSubDomains][; Preload]
            //
            // 需要注意的是，当网站没有使用HSTS时，如果浏览器发现当前网站的证书出现错误，或者浏览器和服务器之间的通信不安全，
            // 无法建立HTTPS连接时，浏览器通常会警告用户，但却又允许用户继续不安全地访问。
            // 而如果网站使用了HSTS，浏览器发现当前连接不安全，则它不仅警告用户，并且不再给用户提供是否继续访问的选择，从而避免后续安全问题的发生
            services.AddHsts(options =>
            {
                // includeSubDomains是可选参数，如果指定这个参数，则表明该网站所有子域名也必须通过HTTPS协议来访问
                options.IncludeSubDomains = true;
                // preload是可选参数，只有在申请将当前网站的域名加入浏览器内置列表时，才需要使用它
                options.Preload = true;
                // max-age参数用来告诉浏览器，在指定时间内，这个网站必须通过HTTPS协议来访问，单位是秒，MaxAge默认值为30天
                options.MaxAge = TimeSpan.FromDays(120);
                // ExcludedHosts属性是一个主机名称列表，出现在该列表中的主机将不会添加Strict-Transport-Security消息头，即不使用HSTS功能，该列表中默认包含3项：localhost、127.0.0.1和[::1]
                options.ExcludedHosts.Clear();
            });
        }
    }
}
