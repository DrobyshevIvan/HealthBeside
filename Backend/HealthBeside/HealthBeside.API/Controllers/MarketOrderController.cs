using System.Security.Claims;
using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Microsoft.AspNetCore.Components.Route("[controller]")]
[ApiController]
public class MarketOrderController : ControllerBase
{
    private readonly IMarketOrderService _marketOrderService;

    public MarketOrderController(IMarketOrderService marketOrderService)
    {
        _marketOrderService = marketOrderService;
    }

    [HttpGet("get-order/{orderId}")]
    [Authorize]
    public async Task<ActionResult<GetOrderDto>> GetOrder(Guid orderId)
    {
        var order = await _marketOrderService.GetOrder(orderId);
        
        return Ok(order);
    }

    [HttpGet("get-user-orders")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<GetOrderDto>>> GetUserOrders()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var orders = await _marketOrderService.GetAllUserOrders(userId);
        
        return Ok(orders);
    }

    [HttpGet("get-orders")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<GetOrderDto>>> GetAllOrders(
        [FromQuery] MarketOrderFilter marketOrderFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams)
    {
        return Ok(await _marketOrderService.GetAllOrders(marketOrderFilter, sortParams, pageParams));
    }

    [HttpPost("add-order")]
    [Authorize]
    public async Task<ActionResult<GetOrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }
        
        var order =  await _marketOrderService.CreateOrder(userId, request.ShippingAddress);
        
        return Ok(order);
    }

    [HttpPut("update-order/{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetOrderDto>> UpdateOrder([FromBody] UpdateOrderRequest request, Guid orderId)
    {
        var updated = await _marketOrderService.UpdateOrder(orderId, request.OrderStatus);
        
        return Ok(updated);
    }

    [HttpDelete("delete-order/{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteOrder(Guid orderId)
    {
        var deleted = await _marketOrderService.DeleteOrder(orderId);
        
        if (!deleted) 
            return Forbid("Order with status delivered cannot be deleted");
        
        return Ok(deleted);
    }
}