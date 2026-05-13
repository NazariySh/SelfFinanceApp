using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SelfFinanceWebApi.Tests.Interfaces
{
    public interface IFinancialReportServiceFactory
    {
        IFinancialReportService CreateService(IDbContextFactory<FinancialDbContext> dbContext);
    }
}
