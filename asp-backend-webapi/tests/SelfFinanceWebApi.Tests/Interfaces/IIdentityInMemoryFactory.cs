namespace SelfFinanceWebApi.Tests.Interfaces
{
    public interface IIdentityInMemoryFactory
    {
        IdentityInMemory CreateIdentityInMemory(string databaseName);
    }
}
