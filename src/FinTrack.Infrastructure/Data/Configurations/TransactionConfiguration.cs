using FinTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        // Campos de auditoria
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt).IsRequired(false);
        builder.Property(t => t.DeletedAt).IsRequired(false);

        builder.Property(t => t.Description).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(t => t.Type).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Date).IsRequired();
        builder.Property(t => t.Status).IsRequired().HasMaxLength(50);

        // Relacionamentos N:1
        builder.HasOne(a => a.Account).WithMany().HasForeignKey(a => a.AccountId).IsRequired();

        // Relacionamento 1:N
        builder.HasOne(a => a.Category).WithMany().HasForeignKey(a => a.CategoryId).IsRequired();

    }
}
