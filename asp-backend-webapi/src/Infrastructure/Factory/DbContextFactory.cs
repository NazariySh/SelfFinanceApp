using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Factory
{
    public class DbContextFactory(DbContextOptions<FinancialDbContext> options) : IDbContextFactory<FinancialDbContext>
    {
        public FinancialDbContext CreateDbContext()
        {
            return new FinancialDbContext(options);
        }
    }
}
