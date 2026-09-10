
using Pos.CatalogService.Application;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Infrastructure.Persistence;
using Pos.CatalogService.Infrastructure.Shared;
using Pos.CatalogService.WebApi.Extensions;
using Pos.CatalogService.WebApi.MiddleWares;
using Pos.CatalogService.WebApi.Policies;
using Pos.CatalogService.WebApi.Services;
using Serilog;

namespace Pos.CatalogService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console()
                 .CreateBootstrapLogger();

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });

            // API Versioning
            builder.Services.AddApiVersioningExtension();

            //-------------------Services Registration-----------------------

            builder.Services.AddPersistenceServices(builder.Configuration);

            builder.Services.AddSharedInfrastructureServices(builder.Configuration);

            builder.Services.AddApplicationLayer(builder.Configuration);

            builder.Services.AddAppPolicies();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddControllers();

            // Authentication and Authorization

            builder.Services.AddAuthenticationServices(builder.Configuration);

            // Swagger (via extension)
            builder.Services.AddSwaggerExtension();

            var app = builder.Build();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerExtension();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
