using Domain.Repository;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using SelfFinanceWebApi.Tests.Interfaces;

namespace SelfFinanceWebApi.Tests.Factory
{
    public class FinancialOperationRepositoryFactory : IFinancialOperationRepositoryFactory
    {
        public IFinancialOperationRepository CreateRepository(IDbContextFactory<FinancialDbContext> dbContext)
        {
            return new FinancialOperationRepository(dbContext);
        }
    }
}
