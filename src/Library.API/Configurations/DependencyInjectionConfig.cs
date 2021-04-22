using Library.API.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IAuthorRepository, AuthorMockRepository>();
            services.AddScoped<IBookRepository, BookMockRepository>();
        }
    }
}
