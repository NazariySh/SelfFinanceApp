using Domain.Repository;
using Infrastructure.Data;
using Infrastructure.Factory;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<FinancialDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton<IDbContextFactory<FinancialDbContext>, DbContextFactory>(_ =>
            {
                return new DbContextFactory(
                        new DbContextOptionsBuilder<FinancialDbContext>()
                        .UseSqlServer(connectionString)
                        .Options);
            });

            services.AddScoped<IFinancialOperationRepository, FinancialOperationRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }
    }
}
