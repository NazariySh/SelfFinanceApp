using Domain.Repository;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SelfFinanceWebApi.Tests.Interfaces
{
    public interface IFinancialOperationRepositoryFactory
    {
        IFinancialOperationRepository CreateRepository(IDbContextFactory<FinancialDbContext> dbContext);
    }
}
