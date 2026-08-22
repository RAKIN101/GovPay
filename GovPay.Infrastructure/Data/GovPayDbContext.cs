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
}