using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SelfFinanceWebApi.Tests.Interfaces
{
    public interface IFinancialOperationServiceFactory
    {
        IFinancialOperationService CreateService(string databaseName);
        IFinancialOperationService CreateService(IDbContextFactory<FinancialDbContext> dbContext);
    }
}
