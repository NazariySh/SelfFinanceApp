using Domain.Repository;

namespace SelfFinanceWebApi.Tests.Interfaces
{
    public interface IAccountRepositoryFactory
    {
        IAccountRepository CreateUserRepository(string databaseName);
    }
}
