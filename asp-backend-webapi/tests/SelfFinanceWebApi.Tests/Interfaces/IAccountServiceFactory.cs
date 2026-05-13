using Application.Interfaces;

namespace SelfFinanceWebApi.Tests.Interfaces
{
    public interface IAccountServiceFactory
    {
        IAccountService CreateAccountService(string database);
    }
}
