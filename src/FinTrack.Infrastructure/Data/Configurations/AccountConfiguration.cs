using FinTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        // Campos de auditoria
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired(false);
        builder.Property(a => a.DeletedAt).IsRequired(false);

        // Precisão decimal
        builder.Property(a => a.InitialBalance).HasPrecision(18, 2);
        builder.Property(a => a.CurrentBalance).HasPrecision(18, 2);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Type).IsRequired().HasMaxLength(50);

        // Relacionamento 1:N
        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).IsRequired();
    }
}
