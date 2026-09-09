using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.CatalogService.Application.Interfaces.Clients;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Infrastructure.Shared.Clients;
using Pos.CatalogService.Infrastructure.Shared.Services;
using Pos.CatalogService.Infrastructure.Shared.Settings;

namespace Pos.CatalogService.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddSharedInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient<ITenantBillingClient, TenantBillingClient>(client =>
            {
                client.BaseAddress = new Uri(
                    configuration["Services:TenantBilling:BaseUrl"]!);
            });

            services.Configure<AzureBlobStorageOptions>(
                configuration.GetSection(AzureBlobStorageOptions.SectionName));

            var storageConnection = configuration["StorageConnection"] ;

            services.AddSingleton(new BlobServiceClient(storageConnection));

            services.AddScoped<IImageStorageService,AzureBlobImageStorageService>();

            return services;
        }
    }
}
