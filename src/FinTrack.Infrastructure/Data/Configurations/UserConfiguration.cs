using FinTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        // Campos de auditoria
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired(false);
        builder.Property(u => u.DeletedAt).IsRequired(false);

        builder.Property(u => u.Name).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(150);

        // Regra de negócio Índice Único
        builder.HasIndex(u => u.Email).IsUnique();

        // PasswordHash (Segurança)
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);

        builder.HasQueryFilter(u => u.DeletedAt == null);
    }

}
