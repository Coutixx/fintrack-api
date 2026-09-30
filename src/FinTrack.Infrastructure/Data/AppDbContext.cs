using FinTrack.Domain.Entities;
using FinTrack.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Data;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.Entity<Account>().HasQueryFilter(a => a.DeletedAt == null);
        modelBuilder.Entity<Category>().HasQueryFilter(c => c.DeletedAt == null);
        modelBuilder.Entity<Transaction>().HasQueryFilter(t => t.DeletedAt == null);
    }
}
