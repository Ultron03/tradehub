namespace TradingPlatform.Domain.Entities;

public enum OrderType
{
    Buy,
    Sell
}

public enum OrderStatus
{
    Pending,
    Completed,
    Failed
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int StockId { get; set; }
    public OrderType Type { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerShare { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Stock Stock { get; set; } = null!;
}
