using System;
using System.Collections.Generic;
using System.Text;

namespace Pos.CatalogService.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
