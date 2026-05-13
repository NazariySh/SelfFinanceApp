using Application.Interfaces;
using Application.Services;
using AutoMapper;
using SelfFinanceWebApi.Tests.Interfaces;

namespace SelfFinanceWebApi.Tests.Factory
{
    public class AccountServiceFactory(
        IMapper mapper,
        IValidatorService validatorService,
        IAccountRepositoryFactory accountRepositoryFactory)
        : IAccountServiceFactory
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IValidatorService _validatorService = validatorService ?? throw new ArgumentNullException(nameof(validatorService));
        private readonly IAccountRepositoryFactory _accountRepositoryFactory = accountRepositoryFactory ?? throw new ArgumentNullException(nameof(accountRepositoryFactory));

        public IAccountService CreateAccountService(string database)
        {
            var accountRepository = _accountRepositoryFactory.CreateUserRepository(database);

            return new AccountService(accountRepository, _validatorService, _mapper);
        }
    }
}
