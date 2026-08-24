using Domain.Repository;
using Infrastructure.Data;
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

            services.AddDbContextFactory<FinancialDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<FinancialDbContext>(sp =>
                sp.GetRequiredService<IDbContextFactory<FinancialDbContext>>().CreateDbContext());

            services.AddScoped<IFinancialOperationRepository, FinancialOperationRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }
    }
}
