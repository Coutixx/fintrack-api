using FinTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(a => a.Id);

        // Campos de auditoria
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired(false);
        builder.Property(a => a.DeletedAt).IsRequired(false);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Type).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Color).IsRequired().HasMaxLength(50);

        builder.HasIndex(a => new { a.UserId, a.Name }).HasFilter("\"DeletedAt\" IS NULL").IsUnique();

        // Relacionamento 1:N
        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).IsRequired();
    }
}
