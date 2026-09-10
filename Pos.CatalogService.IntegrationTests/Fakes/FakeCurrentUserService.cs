using Pos.CatalogService.Application.Interfaces.Services;


namespace Pos.CatalogService.IntegrationTests.Fakes
{
    public class FakeCurrentUserService : ICurrentUserService
    {
        public string? UserId => TestDataFactory.UserId.ToString();

        public Guid? TenantId { get; set; } =
            TestDataFactory.TenantId;

        public string? UserType => "Tenant";

        public IReadOnlyList<string> Roles => new List<string>
        {
            "TenantOwner"
        };

        public string? AccessToken => "fake-access-token";
    }
}
