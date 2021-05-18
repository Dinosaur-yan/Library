using Library.API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.API.Testing
{
    public class CustomWebApplicationFactory<TStartUp> : WebApplicationFactory<Startup>
    {

        /**
         * ConfigureWebHost方法有一个IWebHostBuilder类型的参数，使用该参数的ConfigureServices方法可以配置依赖注入容器，并向容器中添加所需要的服务，
         * 例如在上述代码中所添加的EF Core内存数据库服务。此外，由于在LibraryDbContext类中所重写的方法，在创建内存数据库时也会直接向数据库中添加与之前相同的数据。
         */
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
                services.AddDbContext<LibraryDbContext>(options =>
                {
                    options.UseInMemoryDatabase("LibraryTestingDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<LibraryDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}
