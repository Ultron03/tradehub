namespace TradingPlatform.Application.DTOs;

public record RegisterRequest(string Username, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, string Username, string Email);

public record StockDto(int Id, string Symbol, string Name, decimal CurrentPrice, decimal PreviousClose, decimal Change, decimal ChangePercent);

public record OrderRequest(int StockId, int Quantity);

public record OrderResponse(int OrderId, string Type, string Symbol, int Quantity, decimal PricePerShare, decimal TotalAmount, string Status);

public record PortfolioHoldingDto(string Symbol, string Name, int Quantity, decimal AverageCost, decimal CurrentPrice, decimal MarketValue, decimal ProfitLoss, decimal ProfitLossPercent);

public record PortfolioDto(decimal CashBalance, decimal PortfolioValue, decimal TotalValue, decimal TodaysProfitLoss, decimal TotalProfitLoss, IReadOnlyList<PortfolioHoldingDto> Holdings);

public record TransactionDto(int Id, string Type, string Symbol, string StockName, int Quantity, decimal PricePerShare, decimal TotalAmount, DateTime CreatedAt);
