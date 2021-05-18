using Library.API.Configurations;
using Library.API.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Library.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(configure =>
                {
                    configure.Filters.Add<JsonExceptionFilter>();
                    // 对于请求不支持的数据格式返回406
                    configure.ReturnHttpNotAcceptable = true;
                    // configure.OutputFormatters.Add(new XmlSerializerOutputFormatter()); // 仅支持输出xml格式

                    configure.CacheProfiles.AddCacheProfiles();

                    // 控制器访问添加认证
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    configure.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .AddXmlSerializerFormatters();

            // 数据库上下文
            services.AddDatabaseConfiguration(Configuration);

            // 认证
            services.AddAuthenticationConfiguration(Configuration);

            // 数据保护API
            services.AddDataProtection();

            // AutoMapper
            services.AddAutoMapperConfiguration();

            // 服务器缓存
            services.AddResponseCachingConfiguration();

            // 内存缓存
            services.AddMemoryCacheConfiguration();

            // Redis缓存
            services.AddRedisCacheConfiguration(Configuration);

            // 注入服务
            services.AddDependencyInjectionConfiguration();

            // Swagger
            services.AddSwaggerConfiguration();

            // 版本控制
            services.AddApiVersioningConfiguration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseResponseCachingSetup();

            app.UseMiddleware<RequestRateLimitingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerSetup();
        }
    }
}
