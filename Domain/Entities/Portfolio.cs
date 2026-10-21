namespace TradingPlatform.Domain.Entities;

public class Portfolio
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int StockId { get; set; }
    public int Quantity { get; set; }
    public decimal AverageCost { get; set; }

    public User User { get; set; } = null!;
    public Stock Stock { get; set; } = null!;
}
