using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Library.API.Configurations
{
    public static class AuthenticationConfig
    {
        /**
         * 认证（Authentication）：验证用户身份的过程
         * 授权（Authorization）：验证一个用户是否有权限做某事的过程
         * 1. Basic认证：将用户名、密码以冒号分割，并对组合后的字符串进行Base64编码
         * 2. Digest认证（摘要认证）：客户端根据服务器返回的nonce值、用户的认证信息、请求的URL、使用的http方法以MD5散列值算法计算得到
         * 3. Bearer认证：主要用在OAuth2.0认证，还广泛应用在常见的基于token的认证中
         */
        public static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var tokenSection = configuration.GetSection("Security:Token");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenSection["Issue"],    //合法的签发这
                    ValidAudience = tokenSection["Audience"],   //合法的接收方
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSection["Key"])),   //用于指定进行签名验证的安全密钥
                    ClockSkew = TimeSpan.Zero,   //验证时间的时间偏移值
                };
            });
        }
    }
}
