using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SelfFinanceWebApi.Tests.Factory
{
    public class DbContextFactoryInMemory(string databaseName) : IDbContextFactory<FinancialDbContext>
    {
        private readonly string _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));

        public FinancialDbContext CreateDbContext()
        {
            return new FinancialDbContext(
                    new DbContextOptionsBuilder<FinancialDbContext>()
                       .UseInMemoryDatabase(_databaseName)
                       .Options);
        }
    }
}
