using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pos.CatalogService.Application.Interfaces.Clients;
using Pos.CatalogService.Infrastructure.Shared.Clients;

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

            return services;
        }
    }
}
