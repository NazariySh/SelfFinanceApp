using SelfFinanceWebApi.Tests.Interfaces;

namespace SelfFinanceWebApi.Tests.Factory
{
    public class IdentityInMemoryFactory : IIdentityInMemoryFactory
    {
        public IdentityInMemory CreateIdentityInMemory(string databaseName)
        {
            return new IdentityInMemory(databaseName);
        }
    }
}
