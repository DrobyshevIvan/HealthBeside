using HealthBeside.Application.Contracts.MarketPlace.MarketCategoryDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HealthBeside.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MarketCategoryController : ControllerBase
{
    private readonly IMarketCategoryService _marketCategoryService;
    private readonly ILogger<MarketCategoryController> _logger;

    public MarketCategoryController(IMarketCategoryService marketCategoryService, ILogger<MarketCategoryController> logger)
    {
        _marketCategoryService = marketCategoryService;
        _logger = logger;
    }

    [HttpGet("get-categories")]
    public async Task<ActionResult<IEnumerable<GetMarketCategoryDto>>> GetCategories(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET request to retrieve all market categories.");
        var categories = await _marketCategoryService.GetAllAsync(cancellationToken);
        _logger.LogInformation("Successfully retrieved {Count} categories.", categories.Count());
        return Ok(categories);
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<IEnumerable<GetMarketCategoryDto>>> GetCategory(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET request to retrieve category with ID {CategoryId}.", id);
        try
        {
            var category = await _marketCategoryService.GetByIdAsync(id, cancellationToken);
            _logger.LogInformation("Successfully retrieved category with ID {CategoryId}.", id);
            return Ok(category);
        }
        catch (MarketCategoryException ex)
        {
            _logger.LogWarning(ex, "Category with ID {CategoryId} not found.", id);
            return NotFound(ex.Message);
        }
    }

    [HttpPut("update-category/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdateMarketCategoryDto updateMarketCategoryDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("PUT request to update category with ID {CategoryId}.", id);
        try
        {
            var result = await _marketCategoryService.UpdateAsync(id, updateMarketCategoryDto, cancellationToken);
            _logger.LogInformation("Successfully updated category with ID {CategoryId}.", id);
            return Ok(result);
        }
        catch (MarketCategoryException ex)
        {
            _logger.LogWarning(ex, "Failed to update category with ID {CategoryId}: {Message}", id, ex.Message);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Bad request to update category with ID {CategoryId}: {Message}", id, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("create-category")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetMarketCategoryDto>> CreateCategory(
        [FromBody] CreateMarketCategoryDto createMarketCategoryDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("POST request to create new category with name {CategoryName}.", createMarketCategoryDto.Name);
        try
        {
            var createdCategory = await _marketCategoryService.CreateAsync(createMarketCategoryDto, cancellationToken);
            _logger.LogInformation("Successfully created category with ID {CategoryId}.", createdCategory.Id);
            return CreatedAtAction(nameof(GetCategory), new {id = createdCategory.Id}, createdCategory);
        }
        catch (MarketCategoryException ex)
        {
            _logger.LogError(ex, "Failed to create category with name {CategoryName}: {Message}", createMarketCategoryDto.Name, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("delete-category/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("DELETE request to delete category with ID {CategoryId}.", id);
        
        try
        {
            var result = await _marketCategoryService.DeleteAsync(id, cancellationToken);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting post {PostId}", id);
            return StatusCode(500, "An error occurred while deleting the post.");
        }
    }

    [HttpGet("is-category-exists/{id}")]
    public async Task<ActionResult<bool>> IsCategoryExist(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET request to check if category with ID {CategoryId} exists.", id);
        var exists = await _marketCategoryService.ExistsAsync(id, cancellationToken);
        _logger.LogInformation("Category with ID {CategoryId} existence check returned {Exists}.", id, exists);
        return Ok(exists);
    }
}