using Application.Interfaces;
using Domain.Entities;
using Domain.Repository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SelfFinanceWebApi.Tests.Interfaces;

namespace SelfFinanceWebApi.Tests.Factory
{
    public class AccountRepositoryFactory(IIdentityInMemoryFactory identityInMemoryFactory) : IAccountRepositoryFactory
    {
        private readonly IIdentityInMemoryFactory _identityInMemoryFactory = identityInMemoryFactory ?? throw new ArgumentNullException(nameof(identityInMemoryFactory));

        public IAccountRepository CreateUserRepository(string databaseName)
        {
            var identityInMemory = _identityInMemoryFactory.CreateIdentityInMemory(databaseName);

            var userManager = identityInMemory.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = identityInMemory.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var jwtTokenService = identityInMemory.ServiceProvider.GetRequiredService<IJwtTokenService>();
            var dbContextFactory = new DbContextFactoryInMemory(databaseName);

            return new AccountRepository(userManager, roleManager, jwtTokenService, dbContextFactory);
        }
    }
}
