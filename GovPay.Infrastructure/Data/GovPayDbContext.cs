using GovPay.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GovPay.Infrastructure.Data;

public class GovPayDbContext : DbContext
{
    public GovPayDbContext(DbContextOptions<GovPayDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Bill> Bills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bill>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bills)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}