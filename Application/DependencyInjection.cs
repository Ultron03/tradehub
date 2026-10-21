using Microsoft.Extensions.DependencyInjection;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Application.Services;

namespace TradingPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPortfolioService, PortfolioService>();
        services.AddScoped<ITransactionService, TransactionService>();
        return services;
    }
}
