using System.Security.Claims;
using HealthBeside.Application.Contracts.MarketPlace.MarketCartDto;
using HealthBeside.Application.Contracts.MarketPlace.MarketCartItemDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MarketCartController : ControllerBase
{
    private readonly IMarketCartService _marketCartService;

    public MarketCartController(IMarketCartService marketCartService)
    {
        _marketCartService = marketCartService;
    }

    [HttpGet("get-cart")]
    [Authorize]
    public async Task<ActionResult<GetDetailedCartDto>> GetCart()
    {
        var cartByUserId = await GetUserCart(); 
        
        if(cartByUserId is null)
            return Unauthorized();
        
        var cart = await _marketCartService.GetDetailedCart(cartByUserId.Id);
        return Ok(cart);
    }

    [HttpPost("add-product")]
    [Authorize]
    public async Task<ActionResult<GetCartItemDto>> AddProductToCart([FromBody] AddProductToCartRequest request)
    {
        var cartByUserId = await GetUserCart(); 
        
        if(cartByUserId is null)
            return Unauthorized();
        
        var cartItem = await _marketCartService.AddProductToCart(cartByUserId.Id, request.ProductId, request.Quantity);
        return Ok(cartItem);
        
    }

    [HttpDelete("remove-product/{productId}")]
    [Authorize]
    public async Task<IActionResult> RemoveProductFromCart(Guid productId)
    {
        var cartByUserId = await GetUserCart();
        
        if (cartByUserId is null)
            return Unauthorized();
        
        var result = await _marketCartService.RemoveProductFromCart(cartByUserId.Id, productId);
        if (!result)
            return NotFound();
        
        return Ok(result);
    }

    [HttpPut("update-product")]
    [Authorize]
    public async Task<IActionResult> UpdateProductInCart([FromBody] AddProductToCartRequest request)
    {
        var cartByUserId = await GetUserCart();
        
        if (cartByUserId is null)
            return Unauthorized();
        
        var result = await _marketCartService.UpdateProductInCart(cartByUserId.Id, request.ProductId, request.Quantity);
        
        if (!result)
            return NotFound();
        
        return Ok(result);
    }
    
    private async Task<MarketCart?> GetUserCart()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return await _marketCartService.GetOrCreateCart(userId);
    }
}