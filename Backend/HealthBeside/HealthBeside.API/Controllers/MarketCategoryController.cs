using HealthBeside.Application.Contracts.MarketPlace.MarketCategoryDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Domain.Models.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MarketCategoryController : ControllerBase
{
    private readonly IMarketCategoryService _marketCategoryService;

    public MarketCategoryController(IMarketCategoryService marketCategoryService)
    {
        _marketCategoryService = marketCategoryService;
    }

    [HttpGet("get-categories")]
    public async Task<ActionResult<IEnumerable<GetMarketCategoryDto>>> GetCategories()
    {
        return Ok(await _marketCategoryService.GetAllAsync());
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<IEnumerable<GetMarketCategoryDto>>> GetCategory(Guid id)
    {
        return Ok(await _marketCategoryService.GetByIdAsync(id));
    }

    [HttpPut("update-category/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdateMarketCategoryDto updateMarketCategoryDto)
    {
        var result = await _marketCategoryService.UpdateAsync(id, updateMarketCategoryDto);
        
        if (!result)
            return NotFound($"Post with id {id} not found");
        
        return NoContent();
    }

    [HttpPost("create-category")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetMarketCategoryDto>> CreateCategory(
        [FromBody] CreateMarketCategoryDto createMarketCategoryDto)
    {
        var createdCategory = await _marketCategoryService.CreateAsync(createMarketCategoryDto);
        return CreatedAtAction(nameof(GetCategory), new {id = createdCategory.Id}, createdCategory);
    }

    [HttpDelete("delete-category/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _marketCategoryService.DeleteAsync(id);
        if (!result)
            return NotFound($"Post with id {id} not found");
        
        return NoContent();
    }

    [HttpGet("is-category-exists/{id}")]
    public async Task<ActionResult<bool>> IsCategoryExist(Guid id)
    {
        return await _marketCategoryService.ExistsAsync(id);
    }
}