namespace TradingPlatform.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public decimal CashBalance { get; set; } = 100_000m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Portfolio> Portfolios { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Transaction> Transactions { get; set; } = [];
}
