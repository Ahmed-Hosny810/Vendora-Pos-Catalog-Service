using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pos.CatalogService.Application.Behaviours;
using System.Reflection;


namespace Pos.CatalogService.Application
{
    public static class ApplicationServicesRegistrations
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
                cfg.AddMaps(Assembly.GetExecutingAssembly())
            );
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


        }
    }
}
