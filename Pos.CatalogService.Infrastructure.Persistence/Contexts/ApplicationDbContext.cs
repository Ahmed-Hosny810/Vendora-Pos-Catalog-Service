using Microsoft.EntityFrameworkCore;


namespace Pos.CatalogService.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
