using System.Security.Claims;
using HealthBeside.Application.Contracts;
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
    public async Task<ActionResult<GetOrderDto>> GetOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _marketOrderService.GetOrder(orderId, cancellationToken); 
        
        return Ok(order);
    }

    [HttpGet("get-user-orders")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<GetOrderDto>>> GetUserOrders(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var orders = await _marketOrderService.GetAllUserOrders(userId, cancellationToken);
        
        return Ok(orders);
    }

    [HttpGet("get-orders")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<GetOrderDto>>> GetAllOrders(
        [FromQuery] MarketOrderFilter marketOrderFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _marketOrderService.GetAllOrders(marketOrderFilter, sortParams, pageParams, cancellationToken));
    }

    [HttpPost("add-order")]
    [Authorize]
    public async Task<ActionResult<GetOrderDto>> CreateOrder([FromBody] UserDeliveryInfoDto deliveryInfoDto, CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }
        
        var order =  await _marketOrderService.CreateOrder(userId, deliveryInfoDto, cancellationToken);
        
        return Ok(order);
    }

    [HttpPut("update-order/{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetOrderDto>> UpdateOrder([FromBody] UpdateOrderRequest request, Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var updated = await _marketOrderService.UpdateOrder(orderId, request.OrderStatus, cancellationToken);
        
        return Ok(updated);
    }

    [HttpDelete("delete-order/{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        var deleted = await _marketOrderService.DeleteOrder(orderId, cancellationToken);
        
        if (!deleted) 
            return Forbid("Order with status delivered cannot be deleted");
        
        return Ok(deleted);
    }
}