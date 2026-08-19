using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;
using Pos.CatalogService.Infrastructure.Persistence.UnitofWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Persistence
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => {
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "Catalog");
            }));

            //services.AddScoped(typeof(IGenericRepositoryAsync<,>), typeof(GenericRepositoryAsync<,>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;

        }
    }
}
