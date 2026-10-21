using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.Services;
using TradingPlatform.Domain.Entities;
using TradingPlatform.Infrastructure.Data;

namespace TradingPlatform.Tests;

public class PortfolioServiceTests : IDisposable
{
    private readonly TradingDbContext _db;
    private readonly PortfolioService _portfolioService;
    private readonly User _user;

    public PortfolioServiceTests()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new TradingDbContext(options);
        _portfolioService = new PortfolioService(_db);

        _user = new User { Username = "investor", Email = "investor@test.com", PasswordHash = "hash", CashBalance = 50_000m };
        _db.Users.Add(_user);
        _db.SaveChanges();
    }

    [Fact]
    public async Task GetPortfolio_CalculatesProfitLossCorrectly()
    {
        var msft = new Stock { Symbol = "MSFT", Name = "Microsoft", CurrentPrice = 110m, PreviousClose = 105m };
        var aapl = new Stock { Symbol = "AAPL", Name = "Apple", CurrentPrice = 180m, PreviousClose = 175m };
        _db.Stocks.AddRange(msft, aapl);
        await _db.SaveChangesAsync();

        _db.Portfolios.AddRange(
            new Portfolio { UserId = _user.Id, StockId = msft.Id, Quantity = 10, AverageCost = 100m },
            new Portfolio { UserId = _user.Id, StockId = aapl.Id, Quantity = 5, AverageCost = 170m });
        await _db.SaveChangesAsync();

        var portfolio = await _portfolioService.GetPortfolioAsync(_user.Id);

        Assert.Equal(50_000m, portfolio.CashBalance);
        Assert.Equal(2000m, portfolio.PortfolioValue); // 1100 + 900
        Assert.Equal(52_000m, portfolio.TotalValue);
        Assert.Equal(150m, portfolio.TotalProfitLoss); // (110-100)*10 + (180-170)*5
        Assert.Equal(75m, portfolio.TodaysProfitLoss); // (110-105)*10 + (180-175)*5

        var msftHolding = portfolio.Holdings.First(h => h.Symbol == "MSFT");
        Assert.Equal(100m, msftHolding.ProfitLoss);
        Assert.Equal(10m, msftHolding.ProfitLossPercent);
    }

    [Fact]
    public async Task GetPortfolio_ReturnsEmptyHoldingsForNewUser()
    {
        var portfolio = await _portfolioService.GetPortfolioAsync(_user.Id);

        Assert.Empty(portfolio.Holdings);
        Assert.Equal(0m, portfolio.PortfolioValue);
        Assert.Equal(0m, portfolio.TotalProfitLoss);
    }

    public void Dispose() => _db.Dispose();
}
