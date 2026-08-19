using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddSharedInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            
            return services;
        }
    }
}
