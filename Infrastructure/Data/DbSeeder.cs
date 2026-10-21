using Microsoft.EntityFrameworkCore;
using TradingPlatform.Domain.Entities;
using TradingPlatform.Infrastructure.Data;

namespace TradingPlatform.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(TradingDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (!await context.Stocks.AnyAsync())
        {
            context.Stocks.AddRange(
                new Stock { Symbol = "MSFT", Name = "Microsoft", CurrentPrice = 100.25m, PreviousClose = 99.80m },
                new Stock { Symbol = "AAPL", Name = "Apple", CurrentPrice = 175.50m, PreviousClose = 174.20m },
                new Stock { Symbol = "TSLA", Name = "Tesla", CurrentPrice = 245.30m, PreviousClose = 248.10m },
                new Stock { Symbol = "GOOGL", Name = "Alphabet", CurrentPrice = 142.75m, PreviousClose = 141.90m },
                new Stock { Symbol = "AMZN", Name = "Amazon", CurrentPrice = 178.40m, PreviousClose = 177.25m },
                new Stock { Symbol = "NVDA", Name = "NVIDIA", CurrentPrice = 875.20m, PreviousClose = 868.50m }
            );
            await context.SaveChangesAsync();
        }
    }
}
