using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.DTOs;
using TradingPlatform.Application.Interfaces;

namespace TradingPlatform.Application.Services;

public class TransactionService(ITradingDbContext db) : ITransactionService
{
    public async Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(int userId)
    {
        return await db.Transactions
            .Include(t => t.Stock)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TransactionDto(
                t.Id,
                t.Type.ToString(),
                t.Stock.Symbol,
                t.Stock.Name,
                t.Quantity,
                t.PricePerShare,
                t.TotalAmount,
                t.CreatedAt))
            .ToListAsync();
    }
}
