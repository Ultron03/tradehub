using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.DTOs;
using TradingPlatform.Application.Exceptions;
using TradingPlatform.Application.Interfaces;

namespace TradingPlatform.Application.Services;

public class PortfolioService(ITradingDbContext db) : IPortfolioService
{
    public async Task<PortfolioDto> GetPortfolioAsync(int userId)
    {
        var user = await db.Users.FindAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var holdings = await db.Portfolios
            .Include(p => p.Stock)
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var holdingDtos = holdings.Select(h =>
        {
            var marketValue = h.Stock.CurrentPrice * h.Quantity;
            var costBasis = h.AverageCost * h.Quantity;
            var profitLoss = marketValue - costBasis;
            var profitLossPercent = costBasis == 0 ? 0 : (profitLoss / costBasis) * 100;

            return new PortfolioHoldingDto(
                h.Stock.Symbol,
                h.Stock.Name,
                h.Quantity,
                h.AverageCost,
                h.Stock.CurrentPrice,
                marketValue,
                profitLoss,
                Math.Round(profitLossPercent, 2));
        }).ToList();

        var portfolioValue = holdingDtos.Sum(h => h.MarketValue);
        var totalProfitLoss = holdingDtos.Sum(h => h.ProfitLoss);
        var todaysProfitLoss = holdings.Sum(h => (h.Stock.CurrentPrice - h.Stock.PreviousClose) * h.Quantity);

        return new PortfolioDto(
            user.CashBalance,
            portfolioValue,
            user.CashBalance + portfolioValue,
            todaysProfitLoss,
            totalProfitLoss,
            holdingDtos);
    }
}
