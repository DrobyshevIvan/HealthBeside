using System.Security.Claims;
using HealthBeside.Application.Contracts;
using HealthBeside.Application.Contracts.MarketPlace.MarketOrderDto;
using HealthBeside.Application.Contracts.User;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Microsoft.AspNetCore.Components.Route("[controller]")]
[ApiController]
public class MarketOrderController : ControllerBase
{
    private readonly IMarketOrderService _marketOrderService;
    private readonly ILogger<MarketOrderController> _logger;

    public MarketOrderController(IMarketOrderService marketOrderService, ILogger<MarketOrderController> logger)
    {
        _marketOrderService = marketOrderService;
        _logger = logger;
    }

    [HttpGet("get-order/{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetOrderDto>> GetOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _marketOrderService.GetOrder(orderId, cancellationToken);
            return Ok(order);
        }
        catch (MarketOrderException ex)
        {
            _logger.LogWarning(ex, "Order {OrderId} not found", orderId);
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("get-user-orders")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<GetOrderDto>>> GetUserOrders(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to get user orders");
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
        var orders = await _marketOrderService.GetAllOrders(marketOrderFilter, sortParams, pageParams, cancellationToken);
        return Ok(orders);
    }

    [HttpPost("create-order")]
    [Authorize]
    public async Task<ActionResult<GetOrderDto>> CreateOrder(
        [FromBody] UserDeliveryInfoDto deliveryInfoDto,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized access attempt to create order");
            return Unauthorized();
        }

        try
        {
            _logger.LogInformation("User {UserId} is attempting to create a new order", userId);

            var order = await _marketOrderService.CreateOrder(userId, deliveryInfoDto, cancellationToken);

            _logger.LogInformation("Order {OrderId} successfully created for user {UserId}", order.Id, userId);

            return Ok(order);
        }
        catch (MarketOrderException ex)
        {
            _logger.LogWarning(ex, "Business validation failed for user {UserId} when creating order", userId);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating order for user {UserId}", userId);
            return StatusCode(500, new { message = "An unexpected error occurred while creating the order." });
        }
    }

    [HttpPut("update-order/{orderId}")]
    [Authorize]
    public async Task<ActionResult<GetOrderDto>> UpdateOrder(Guid orderId, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var updated = await _marketOrderService.UpdateOrder(orderId, request.OrderStatus, cancellationToken);
            return Ok(updated);
        }
        catch (MarketOrderException ex)
        {
            _logger.LogWarning(ex, "Failed to update order {OrderId}", orderId);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("delete-order/{orderId}")]
    [Authorize]
    public async Task<ActionResult> DeleteOrder(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _marketOrderService.DeleteOrder(orderId, cancellationToken);

            if (!deleted)
                return Forbid("Order with status delivered cannot be deleted");

            return Ok(new { deleted = true });
        }
        catch (MarketOrderException ex)
        {
            _logger.LogWarning(ex, "Failed to delete order {OrderId}", orderId);
            return NotFound(new { error = ex.Message });
        }
    }
}