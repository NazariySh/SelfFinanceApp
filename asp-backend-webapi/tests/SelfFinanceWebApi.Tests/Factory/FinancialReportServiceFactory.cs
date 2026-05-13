using Application.Interfaces;
using Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SelfFinanceWebApi.Tests.Interfaces;

namespace SelfFinanceWebApi.Tests.Factory
{
    public class FinancialReportServiceFactory(IFinancialOperationServiceFactory operationServiceFactory) : IFinancialReportServiceFactory
    {
        private readonly IFinancialOperationServiceFactory _operationServiceFactory = operationServiceFactory ?? throw new ArgumentNullException(nameof(operationServiceFactory));

        public IFinancialReportService CreateService(IDbContextFactory<FinancialDbContext> dbContext)
        {
            var operationService = _operationServiceFactory.CreateService(dbContext);

            return new FinancialReportService(operationService);
        }
    }
}
