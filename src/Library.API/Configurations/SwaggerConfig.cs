using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Library.API.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Library Project",
                    Description = "Library API Swagger surface"
                });

                c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "swagger", "Library.API.xml"), true);

                var security = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                };

                c.AddSecurityRequirement(security);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Format: {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });
            });
        }

        public static void UseSwaggerSetup(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
