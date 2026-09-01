using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;
using Pos.CatalogService.Infrastructure.Persistence.Repositories;
using Pos.CatalogService.Infrastructure.Persistence.UnitofWork;


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

            services.AddScoped(typeof(IGenericRepositoryAsync<,>), typeof(GenericRepositoryAsync<,>));

            services.AddScoped<ITaxRateRepositoryAsync, TaxRateRepositoryAsync>();

            services.AddScoped<IProductVariantRepositoryAsync, ProductVariantRepositoryAsync>();

            services.AddScoped<IProductRepositoryAsync, ProductRepositoryAsync>();

            services.AddScoped<IUnitRepositoryAsync, UnitRepositoryAsync>();

            services.AddScoped<ICategoryRepositoryAsync, CategoryRepositoryAsync>();

            services.AddScoped<IProductImageRepositoryAsync, ProductImageRepositoryAsync>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;

        }
    }
}
