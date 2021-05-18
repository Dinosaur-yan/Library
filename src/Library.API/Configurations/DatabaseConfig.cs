using Library.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Configurations
{
    public static class DatabaseConfig
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDbContext<LibraryDbContext>(options =>
            {
                // 第二个参数使用MySqlDbContextOptionsBuilder对象的MigrationsAssembly方法为当前DbContext设置其迁移所在的程序集名称
                // 这是由于DbContext与为其创建的迁移并不在同一程序集中  使用Add-Migration AddIdentity Update-Database创建迁移
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                    optionBuilder => optionBuilder.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name));
            });

            // 此服务应添加在Authentication之前，以避免该方法中添加的认证方式替换我们添加的jwt bearer默认方式
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<LibraryDbContext>();
        }
    }
}
