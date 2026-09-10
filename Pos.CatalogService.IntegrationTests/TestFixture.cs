using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pos.CatalogService.Application.Features.Categories.Commands.CreateCommand;
using Pos.CatalogService.Application.Interfaces;
using Pos.CatalogService.Application.Interfaces.Clients;
using Pos.CatalogService.Application.Interfaces.Repositories;
using Pos.CatalogService.Application.Interfaces.Services;
using Pos.CatalogService.Application.Settings;
using Pos.CatalogService.Infrastructure.Persistence.Contexts;
using Pos.CatalogService.Infrastructure.Persistence.Repositories;
using Pos.CatalogService.Infrastructure.Persistence.UnitofWork;
using Pos.CatalogService.IntegrationTests.Fakes;


namespace Pos.CatalogService.IntegrationTests
{
    public class TestFixture
    {
        public ServiceProvider ServiceProvider { get; }

        public ApplicationDbContext DbContext { get; }

        public IMediator Mediator { get; }

        public FakeCurrentUserService CurrentUserService { get; }

        public FakeTenantBillingClient TenantBillingClient { get; }

        public FakeImageStorageService ImageStorageService { get; }

        public TestFixture()
        {
            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Warning);
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(
                    $"CatalogServiceTestDb_{Guid.NewGuid()}");
            });

            /*
                Register MediatR handlers manually from the
                Catalog Application assembly.
            */
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(
                    typeof(CreateCategoryCommandHandler).Assembly);
            });

            /*
                Register the real repositories.
            */
            services.AddScoped<
                ICategoryRepositoryAsync,
                CategoryRepositoryAsync>();

            services.AddScoped<
                IUnitRepositoryAsync,
                UnitRepositoryAsync>();

            services.AddScoped<
                ITaxRateRepositoryAsync,
                TaxRateRepositoryAsync>();

            services.AddScoped<
                IProductRepositoryAsync,
                ProductRepositoryAsync>();

            services.AddScoped<
                IProductVariantRepositoryAsync,
                ProductVariantRepositoryAsync>();

            services.AddScoped<
                IProductImageRepositoryAsync,
                ProductImageRepositoryAsync>();

            services.AddScoped<
                IImageUploadSessionRepositoryAsync,
                ImageUploadSessionRepositoryAsync>();

            services.AddScoped(
                typeof(IGenericRepositoryAsync<,>),
                typeof(GenericRepositoryAsync<,>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            /*
                Configure product-image upload rules.
            */
            services.Configure<ProductImageUploadSettings>(options =>
            {
                options.MaxSizeBytes = 5 * 1024 * 1024;
                options.UploadExpirationMinutes = 10;

                options.AllowedContentTypes =
                [
                    "image/jpeg",
                    "image/png",
                    "image/webp"
                ];

                options.AllowedExtensions =
                [
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                ];
            });

            /*
                Replace request and external infrastructure
                dependencies with controlled fakes.
            */
            CurrentUserService = new FakeCurrentUserService();
            TenantBillingClient = new FakeTenantBillingClient();
            ImageStorageService = new FakeImageStorageService();

            services.AddSingleton<ICurrentUserService>(
                CurrentUserService);

            services.AddSingleton<ITenantBillingClient>(
                TenantBillingClient);

            services.AddSingleton<IImageStorageService>(
                ImageStorageService);

            ServiceProvider = services.BuildServiceProvider();

            DbContext =
                ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Mediator =
                ServiceProvider.GetRequiredService<IMediator>();

            DbContext.Database.EnsureCreated();
        }

        public async Task SaveChangesAndClearAsync()
        {
            await DbContext.SaveChangesAsync();

            /*
                Simulate a new request scope after test data
                has been inserted.
            */
            DbContext.ChangeTracker.Clear();
        }
    }
}
