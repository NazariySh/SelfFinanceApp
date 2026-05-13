using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class FinancialDbContext(DbContextOptions<FinancialDbContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<FinancialOperation> FinancialOperations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FinancialOperationConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
