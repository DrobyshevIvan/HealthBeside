using System.Security.Claims;
using HealthBeside.Application.Contracts.MarketPlace.MarketCartDto;
using HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarketCartController : ControllerBase
{
    private readonly IMarketCartService _marketCartService;
    private readonly ILogger<MarketCartController> _logger;

    public MarketCartController(IMarketCartService marketCartService, ILogger<MarketCartController> logger)
    {
        _marketCartService = marketCartService;
        _logger = logger;
    }

    [HttpGet("get-cart")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<GetDetailedCartDto>> GetCart()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Unauthorized cart access attempt.");
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var cart = await _marketCartService.GetOrCreateCart(userId);

            var detailedCart = await _marketCartService.GetDetailedCart(cart.Id, userId, isAdmin);
            _logger.LogInformation("Cart retrieved successfully for user {UserId}", userId);
            return Ok(detailedCart);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("User {UserId} tried to access cart without permission", userId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the cart.");
        }
    }


    [HttpPost("add-product")]
    [Authorize]
    public async Task<ActionResult<GetCartItemDto>> AddProductToCart([FromBody] AddProductToCartRequest request)
    {
        if (request.Quantity <= 0)
            return BadRequest("Quantity must be greater than zero.");

        var cart = await GetUserCart();
        if (cart is null)
            return Unauthorized();

        try
        {
            var cartItem = await _marketCartService.AddProductToCart(cart.Id, request.ProductId, request.Quantity);
            _logger.LogInformation("Product {ProductId} added to cart {CartId}", request.ProductId, cart.Id);
            return Ok(cartItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product {ProductId} to cart {CartId}", request.ProductId, cart.Id);
            return StatusCode(500, "An error occurred while adding product to cart.");
        }
    }

    [HttpDelete("remove-product/{productId}")]
    [Authorize]
    public async Task<IActionResult> RemoveProductFromCart(Guid productId)
    {
        var cart = await GetUserCart();
        if (cart is null)
            return Unauthorized();

        var result = await _marketCartService.RemoveProductFromCart(cart.Id, productId);
        if (!result)
        {
            _logger.LogWarning("Product {ProductId} not found in cart {CartId}", productId, cart.Id);
            return NotFound();
        }

        _logger.LogInformation("Product {ProductId} removed from cart {CartId}", productId, cart.Id);
        return Ok(result);
    }

    [HttpPut("update-product")]
    [Authorize]
    public async Task<ActionResult<GetDetailedCartDto>> UpdateProductInCart([FromBody] AddProductToCartRequest request)
    {
        if (request.Quantity <= 0)
            return BadRequest("Quantity must be greater than zero.");

        var cart = await GetUserCart();
        if (cart is null)
            return Unauthorized();
        
        try
        {
            var result = await _marketCartService.UpdateProductInCart(cart.Id, request.ProductId, request.Quantity);
            _logger.LogInformation("Product {ProductId} updated in cart {CartId}", request.ProductId, cart.Id);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogWarning("Product {ProductId} not found in cart {CartId}", request.ProductId, cart.Id);
            return NotFound();
        }
    }

    private async Task<MarketCart?> GetUserCart()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return null;

        return await _marketCartService.GetOrCreateCart(userId);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
