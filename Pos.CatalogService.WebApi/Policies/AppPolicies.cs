using OpenIddict.Validation.AspNetCore;
using Pos.CatalogService.Application.Common.Constants;

namespace Pos.CatalogService.WebApi.Policies
{
    public static class AppPolicies
    {
        public static IServiceCollection AddAppPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(CatalogPolicies.CanViewCatalog, policy =>
                {
                    policy.AuthenticationSchemes.Add(
                        OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

                    policy.RequireAuthenticatedUser();

                    policy.RequireRole(
                        CatalogRoles.TenantOwner,
                        CatalogRoles.Admin,
                        CatalogRoles.Cashier,
                        CatalogRoles.InventoryStaff
                        );
                });

                options.AddPolicy(CatalogPolicies.CanManageCatalog, policy =>
                {
                    policy.AuthenticationSchemes.Add(
                        OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

                    policy.RequireAuthenticatedUser();

                    policy.RequireRole(
                        CatalogRoles.TenantOwner,
                        CatalogRoles.Admin,
                        CatalogRoles.InventoryStaff);
                });

            });

            return services;
        }
    }
}
