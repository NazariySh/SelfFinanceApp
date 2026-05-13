using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal sealed class FinancialOperationConfiguration : IEntityTypeConfiguration<FinancialOperation>
    {
        public void Configure(EntityTypeBuilder<FinancialOperation> builder)
        {
            builder.ToTable("FinancialOperations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasOne<AppUser>()
                .WithMany(u => u.FinancialOperations)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        }
    }
}
