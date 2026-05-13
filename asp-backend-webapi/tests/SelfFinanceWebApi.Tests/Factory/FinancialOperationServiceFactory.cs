using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SelfFinanceWebApi.Tests.Interfaces;

namespace SelfFinanceWebApi.Tests.Factory
{
    public class FinancialOperationServiceFactory(
        IMapper mapper,
        IValidatorService validatorService,
        IFinancialOperationRepositoryFactory operationRepository)
        : IFinancialOperationServiceFactory
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IValidatorService _validatorService = validatorService ?? throw new ArgumentNullException(nameof(validatorService));
        private readonly IFinancialOperationRepositoryFactory _operationRepositoryFactory = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));

        public IFinancialOperationService CreateService(string databaseName)
        {
            var dbContextFactory = new DbContextFactoryInMemory(databaseName);

            return CreateService(dbContextFactory);
        }

        public IFinancialOperationService CreateService(IDbContextFactory<FinancialDbContext> dbContext)
        {
            var repository = _operationRepositoryFactory.CreateRepository(dbContext);

            return new FinancialOperationService(repository, _validatorService, _mapper);
        }
    }
}
