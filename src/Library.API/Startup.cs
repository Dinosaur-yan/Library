using Library.API.Configurations;
using Library.API.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                })
                .AddNewtonsoftJson()
                .AddXmlSerializerFormatters();

            services.AddDatabaseConfiguration(Configuration);

            services.AddAutoMapperConfiguration();

            services.AddSwaggerConfiguration();

            services.AddResponseCachingConfiguration();

            services.AddMemoryCacheConfiguration();

            services.AddRedisCacheConfiguration(Configuration);

            services.AddDependencyInjectionConfiguration();

            services.AddApiVersioningConfiguration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseResponseCachingSetup();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerSetup();
        }
    }
}
