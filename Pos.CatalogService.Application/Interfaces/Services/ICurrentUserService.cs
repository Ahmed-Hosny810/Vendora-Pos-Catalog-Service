using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }

        Guid? TenantId { get; }

        string? UserType { get; }

        IReadOnlyList<string> Roles { get; }

        string? AccessToken { get; }
    }
}
