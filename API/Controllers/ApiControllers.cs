using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingPlatform.Application.DTOs;
using TradingPlatform.Application.Interfaces;

namespace TradingPlatform.API.Controllers;

[ApiController]
[Route("")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        return Ok(await authService.RegisterAsync(request));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        return Ok(await authService.LoginAsync(request));
    }
}

[ApiController]
[Route("stocks")]
public class StocksController(IStockService stockService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StockDto>>> GetAll()
    {
        return Ok(await stockService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockDto>> GetById(int id)
    {
        return Ok(await stockService.GetByIdAsync(id));
    }
}

[ApiController]
[Route("orders")]
[Authorize]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost("buy")]
    public async Task<ActionResult<OrderResponse>> Buy([FromBody] OrderRequest request)
    {
        return Ok(await orderService.BuyAsync(GetUserId(), request));
    }

    [HttpPost("sell")]
    public async Task<ActionResult<OrderResponse>> Sell([FromBody] OrderRequest request)
    {
        return Ok(await orderService.SellAsync(GetUserId(), request));
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

[ApiController]
[Route("portfolio")]
[Authorize]
public class PortfolioController(IPortfolioService portfolioService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PortfolioDto>> Get()
    {
        return Ok(await portfolioService.GetPortfolioAsync(GetUserId()));
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

[ApiController]
[Route("transactions")]
[Authorize]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> Get()
    {
        return Ok(await transactionService.GetTransactionsAsync(GetUserId()));
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
