using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MarketProductController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMarketProductService _marketProductService;

    public MarketProductController(AppDbContext context,
        IMarketProductService marketProductService)
    {
        _context = context;
        _marketProductService = marketProductService;
    }

    [HttpGet("get-products")]
    public async Task<ActionResult<IEnumerable<GetMarketProductDto>>> GetMarketProducts()
    {
        return Ok(await _marketProductService.GetAllAsync());
    }

    [HttpGet("get-product/{id}")]
    public async Task<ActionResult<GetDetailedMarketProductDto>> GetMarketProduct(Guid id)
    {
        return Ok(await _marketProductService.GetByIdAsync(id));
    }

    [HttpPut("update-product/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMarketProductDto updateForumPostDto)
    {
        var result = await _marketProductService.UpdateAsync(id, updateForumPostDto);
        
        if (!result)
            return NotFound($"Post with id {id} not found.");
        
        return NoContent();
    }

    [HttpPost("create-product")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetDetailedMarketProductDto>> CreateProduct(
        [FromBody] CreateMarketProductDto createDto)
    {
        var createdProduct = await _marketProductService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetMarketProduct), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpDelete("delete-product/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await _marketProductService.DeleteAsync(id);
        if (!result)
            return NotFound($"Product with id {id} not found.");
        
        return NoContent();
    }

    [HttpGet("is-product-exist/{id}")]
    private async Task<bool> ProductExist(Guid id)
    {
        return await _marketProductService.ExistsAsync(id);
    }
}