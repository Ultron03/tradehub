using Microsoft.EntityFrameworkCore;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Application.Interfaces;

public interface ITradingDbContext
{
    DbSet<User> Users { get; }
    DbSet<Stock> Stocks { get; }
    DbSet<Order> Orders { get; }
    DbSet<Portfolio> Portfolios { get; }
    DbSet<Transaction> Transactions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
