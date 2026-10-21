using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.DTOs;
using TradingPlatform.Application.Exceptions;
using TradingPlatform.Application.Services;
using TradingPlatform.Domain.Entities;
using TradingPlatform.Infrastructure.Data;

namespace TradingPlatform.Tests;

public class OrderServiceTests : IDisposable
{
    private readonly TradingDbContext _db;
    private readonly OrderService _orderService;
    private readonly User _user;
    private readonly Stock _stock;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new TradingDbContext(options);
        _orderService = new OrderService(_db);

        _stock = new Stock { Symbol = "MSFT", Name = "Microsoft", CurrentPrice = 100m, PreviousClose = 99m };
        _user = new User { Username = "trader", Email = "trader@test.com", PasswordHash = "hash", CashBalance = 10_000m };

        _db.Stocks.Add(_stock);
        _db.Users.Add(_user);
        _db.SaveChanges();
    }

    [Fact]
    public async Task BuyOrder_ReducesCashAndAddsHolding()
    {
        var response = await _orderService.BuyAsync(_user.Id, new OrderRequest(_stock.Id, 10));

        Assert.Equal("Buy", response.Type);
        Assert.Equal("Completed", response.Status);
        Assert.Equal(10, response.Quantity);
        Assert.Equal(1000m, response.TotalAmount);

        var updatedUser = await _db.Users.FindAsync(_user.Id);
        Assert.Equal(9000m, updatedUser!.CashBalance);

        var holding = await _db.Portfolios.FirstAsync();
        Assert.Equal(10, holding.Quantity);
        Assert.Equal(100m, holding.AverageCost);
    }

    [Fact]
    public async Task BuyOrder_ThrowsWhenInsufficientFunds()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _orderService.BuyAsync(_user.Id, new OrderRequest(_stock.Id, 200)));

        Assert.Contains("Insufficient funds", ex.Message);
    }

    [Fact]
    public async Task SellOrder_IncreasesCashAndReducesHolding()
    {
        _db.Portfolios.Add(new Portfolio { UserId = _user.Id, StockId = _stock.Id, Quantity = 20, AverageCost = 95m });
        await _db.SaveChangesAsync();

        var response = await _orderService.SellAsync(_user.Id, new OrderRequest(_stock.Id, 5));

        Assert.Equal("Sell", response.Type);
        Assert.Equal(500m, response.TotalAmount);

        var updatedUser = await _db.Users.FindAsync(_user.Id);
        Assert.Equal(10_500m, updatedUser!.CashBalance);

        var holding = await _db.Portfolios.FirstAsync();
        Assert.Equal(15, holding.Quantity);
    }

    [Fact]
    public async Task SellOrder_ThrowsWhenInsufficientShares()
    {
        _db.Portfolios.Add(new Portfolio { UserId = _user.Id, StockId = _stock.Id, Quantity = 3, AverageCost = 95m });
        await _db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _orderService.SellAsync(_user.Id, new OrderRequest(_stock.Id, 10)));

        Assert.Contains("Insufficient shares", ex.Message);
    }

    [Fact]
    public async Task SellOrder_ThrowsWhenNoHolding()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _orderService.SellAsync(_user.Id, new OrderRequest(_stock.Id, 1)));

        Assert.Contains("Insufficient shares", ex.Message);
    }

    public void Dispose() => _db.Dispose();
}
