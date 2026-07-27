using FinTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations;

public class AccountConfiguration
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        // Campos de auditoria
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired(false);
        builder.Property(u => u.DeletedAt).IsRequired(false);

        // Precisão decimal
        builder.Property(a => a.InitialBalance).HasPrecision(18, 2);
        builder.Property(a => a.CurrentBalance).HasPrecision(18, 2);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Type).IsRequired().HasMaxLength(50);

        // Relacionamento 1:N
        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).IsRequired(false);
    }
}
