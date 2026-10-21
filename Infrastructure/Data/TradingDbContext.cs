using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Infrastructure.Data;

public class TradingDbContext(DbContextOptions<TradingDbContext> options) : DbContext(options), ITradingDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.CashBalance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasIndex(s => s.Symbol).IsUnique();
            entity.Property(s => s.CurrentPrice).HasPrecision(18, 2);
            entity.Property(s => s.PreviousClose).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.PricePerShare).HasPrecision(18, 2);
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Portfolio>(entity =>
        {
            entity.HasIndex(p => new { p.UserId, p.StockId }).IsUnique();
            entity.Property(p => p.AverageCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.PricePerShare).HasPrecision(18, 2);
            entity.Property(t => t.TotalAmount).HasPrecision(18, 2);
        });
    }
}
