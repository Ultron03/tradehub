using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradingPlatform.Infrastructure.Data;

namespace TradingPlatform.Infrastructure.Services;

public class PriceSimulationService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly Random _random = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
                var stocks = await db.Stocks.ToListAsync(stoppingToken);

                foreach (var stock in stocks)
                {
                    var changePercent = (_random.NextDouble() - 0.5) * 0.02;
                    var newPrice = Math.Round(stock.CurrentPrice * (1 + (decimal)changePercent), 2);
                    stock.CurrentPrice = Math.Max(newPrice, 0.01m);
                    stock.LastUpdated = DateTime.UtcNow;
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Log and continue - price simulation should not crash the app
            }

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}
