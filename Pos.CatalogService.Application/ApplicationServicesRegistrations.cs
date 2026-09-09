using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pos.CatalogService.Application.Behaviours;
using Pos.CatalogService.Application.Settings;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Pos.CatalogService.Application
{
    public static class ApplicationServicesRegistrations
    {
        public static void AddApplicationLayer(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAutoMapper(cfg =>
                cfg.AddMaps(Assembly.GetExecutingAssembly())
            );
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.Configure<ProductImageUploadSettings>(
                configuration.GetSection(ProductImageUploadSettings.SectionName));

        }
    }
}
