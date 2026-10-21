using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.DTOs;
using TradingPlatform.Application.Exceptions;
using TradingPlatform.Application.Interfaces;

namespace TradingPlatform.Application.Services;

public class StockService(ITradingDbContext db) : IStockService
{
    public async Task<IReadOnlyList<StockDto>> GetAllAsync()
    {
        var stocks = await db.Stocks.OrderBy(s => s.Symbol).ToListAsync();
        return stocks.Select(MapToDto).ToList();
    }

    public async Task<StockDto> GetByIdAsync(int id)
    {
        var stock = await db.Stocks.FindAsync(id)
            ?? throw new NotFoundException($"Stock with id {id} not found.");
        return MapToDto(stock);
    }

    internal static StockDto MapToDto(Domain.Entities.Stock stock)
    {
        var change = stock.CurrentPrice - stock.PreviousClose;
        var changePercent = stock.PreviousClose == 0 ? 0 : (change / stock.PreviousClose) * 100;
        return new StockDto(stock.Id, stock.Symbol, stock.Name, stock.CurrentPrice, stock.PreviousClose, change, Math.Round(changePercent, 2));
    }
}
