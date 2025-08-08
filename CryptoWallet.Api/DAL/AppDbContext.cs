using CryptoWallet.Api.Dal.Configurations;
using CryptoWallet.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWallet.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

    public DbSet<User> Users => Set<User>();

}
