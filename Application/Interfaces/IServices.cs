using TradingPlatform.Application.DTOs;
using TradingPlatform.Domain.Entities;

namespace TradingPlatform.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

public interface IStockService
{
    Task<IReadOnlyList<StockDto>> GetAllAsync();
    Task<StockDto> GetByIdAsync(int id);
}

public interface IOrderService
{
    Task<OrderResponse> BuyAsync(int userId, OrderRequest request);
    Task<OrderResponse> SellAsync(int userId, OrderRequest request);
}

public interface IPortfolioService
{
    Task<PortfolioDto> GetPortfolioAsync(int userId);
}

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(int userId);
}

public interface IJwtTokenService
{
    string GenerateToken(User user);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
