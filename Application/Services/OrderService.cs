using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.DTOs;
using TradingPlatform.Application.Exceptions;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Application.Services;

public class OrderService(ITradingDbContext db) : IOrderService
{
    public async Task<OrderResponse> BuyAsync(int userId, OrderRequest request)
    {
        ValidateOrderRequest(request);

        var user = await db.Users.FindAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var stock = await db.Stocks.FindAsync(request.StockId)
            ?? throw new NotFoundException($"Stock with id {request.StockId} not found.");

        var totalAmount = stock.CurrentPrice * request.Quantity;
        if (user.CashBalance < totalAmount)
        {
            throw new ValidationException($"Insufficient funds. Required ₹{totalAmount:N2}, available ₹{user.CashBalance:N2}.");
        }

        user.CashBalance -= totalAmount;

        var holding = await db.Portfolios.FirstOrDefaultAsync(p => p.UserId == userId && p.StockId == request.StockId);
        if (holding is null)
        {
            db.Portfolios.Add(new Portfolio
            {
                UserId = userId,
                StockId = request.StockId,
                Quantity = request.Quantity,
                AverageCost = stock.CurrentPrice
            });
        }
        else
        {
            var newTotalCost = (holding.AverageCost * holding.Quantity) + totalAmount;
            holding.Quantity += request.Quantity;
            holding.AverageCost = newTotalCost / holding.Quantity;
        }

        var order = new Order
        {
            UserId = userId,
            StockId = request.StockId,
            Type = OrderType.Buy,
            Quantity = request.Quantity,
            PricePerShare = stock.CurrentPrice,
            TotalAmount = totalAmount,
            Status = OrderStatus.Completed
        };
        db.Orders.Add(order);

        db.Transactions.Add(new Transaction
        {
            UserId = userId,
            StockId = request.StockId,
            Type = TransactionType.Buy,
            Quantity = request.Quantity,
            PricePerShare = stock.CurrentPrice,
            TotalAmount = totalAmount
        });

        await db.SaveChangesAsync();

        return MapToResponse(order, stock.Symbol);
    }

    public async Task<OrderResponse> SellAsync(int userId, OrderRequest request)
    {
        ValidateOrderRequest(request);

        var user = await db.Users.FindAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var stock = await db.Stocks.FindAsync(request.StockId)
            ?? throw new NotFoundException($"Stock with id {request.StockId} not found.");

        var holding = await db.Portfolios.FirstOrDefaultAsync(p => p.UserId == userId && p.StockId == request.StockId);
        if (holding is null || holding.Quantity < request.Quantity)
        {
            var owned = holding?.Quantity ?? 0;
            throw new ValidationException($"Insufficient shares to sell. You own {owned} shares, tried to sell {request.Quantity}.");
        }

        var totalAmount = stock.CurrentPrice * request.Quantity;
        user.CashBalance += totalAmount;

        holding.Quantity -= request.Quantity;
        if (holding.Quantity == 0)
        {
            db.Portfolios.Remove(holding);
        }

        var order = new Order
        {
            UserId = userId,
            StockId = request.StockId,
            Type = OrderType.Sell,
            Quantity = request.Quantity,
            PricePerShare = stock.CurrentPrice,
            TotalAmount = totalAmount,
            Status = OrderStatus.Completed
        };
        db.Orders.Add(order);

        db.Transactions.Add(new Transaction
        {
            UserId = userId,
            StockId = request.StockId,
            Type = TransactionType.Sell,
            Quantity = request.Quantity,
            PricePerShare = stock.CurrentPrice,
            TotalAmount = totalAmount
        });

        await db.SaveChangesAsync();

        return MapToResponse(order, stock.Symbol);
    }

    private static void ValidateOrderRequest(OrderRequest request)
    {
        if (request.StockId <= 0)
        {
            throw new ValidationException("A valid stock is required.");
        }

        if (request.Quantity <= 0)
        {
            throw new ValidationException("Quantity must be greater than zero.");
        }
    }

    private static OrderResponse MapToResponse(Order order, string symbol) =>
        new(order.Id, order.Type.ToString(), symbol, order.Quantity, order.PricePerShare, order.TotalAmount, order.Status.ToString());
}
