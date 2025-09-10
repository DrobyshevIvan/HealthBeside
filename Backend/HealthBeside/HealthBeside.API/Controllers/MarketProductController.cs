using System.Security.Claims;
using HealthBeside.Application.Contracts.MarketPlace.MarketProductDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using HealthBeside.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarketProductController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMarketProductService _marketProductService;
    private readonly ILogger<MarketProductController> _logger;

    public MarketProductController(
        AppDbContext context,
        IMarketProductService marketProductService,
        ILogger<MarketProductController> logger)
    {
        _context = context;
        _marketProductService = marketProductService;
        _logger = logger;
    }

    [HttpGet("get-products")]
    public async Task<ActionResult<IEnumerable<GetMarketProductDto>>> GetMarketProducts(
        [FromQuery] MarketProductFilter marketProductFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /get-products called with filters: {@Filters}, sort: {@Sort}, page: {@Page}",
            marketProductFilter, sortParams, pageParams);

        var result = await _marketProductService.GetAllAsync(marketProductFilter, sortParams, pageParams, cancellationToken);
        _logger.LogInformation("Returned {Count} market products.", result.Products.Count());
        return Ok(result);
    }

    [HttpGet("get-product/{id}")]
    public async Task<ActionResult<GetDetailedMarketProductDto>> GetMarketProduct(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /get-product/{ProductId} called.", id);
        var product = await _marketProductService.GetByIdAsync(id, cancellationToken);
        return Ok(product);
    }

    [HttpPut("update-product/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetMarketProductDto>> Update(
        Guid id,
        [FromBody] UpdateMarketProductDto updateDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("PUT /update-product/{ProductId} called by user {UserId}", id, User.FindFirstValue(ClaimTypes.NameIdentifier));

        try
        {
            var result = await _marketProductService.UpdateAsync(id, updateDto, cancellationToken);
            _logger.LogInformation("Market product {ProductId} updated successfully.", id);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogWarning("Market product {ProductId} not found during update.", id);
            return NotFound($"Product with id {id} not found.");
        }
    }

    [HttpPost("create-product")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetDetailedMarketProductDto>> CreateProduct(
        [FromBody] CreateMarketProductDto createDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("POST /create-product called by user {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));

        var createdProduct = await _marketProductService.CreateAsync(createDto, cancellationToken);

        _logger.LogInformation("Market product {ProductId} created successfully.", createdProduct.Id);
        return CreatedAtAction(nameof(GetMarketProduct), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpDelete("delete-product/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("DELETE /delete-product/{ProductId} called by user {UserId}", id, User.FindFirstValue(ClaimTypes.NameIdentifier));

        var result = await _marketProductService.DeleteAsync(id, cancellationToken);

        if (!result)
        {
            _logger.LogWarning("Market product {ProductId} not found for deletion.", id);
            return NotFound($"Product with id {id} not found.");
        }

        _logger.LogInformation("Market product {ProductId} deleted successfully.", id);
        return NoContent();
    }

    [HttpGet("is-product-exist/{id}")]
    private async Task<bool> ProductExist(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking existence of market product {ProductId}", id);
        return await _marketProductService.ExistsAsync(id, cancellationToken);
    }
}
