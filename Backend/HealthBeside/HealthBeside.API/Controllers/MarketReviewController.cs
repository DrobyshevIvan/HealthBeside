using HealthBeside.Application.Contracts.MarketPlace.MarketReviewDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MarketReviewController : ControllerBase
{
    private readonly IMarketReviewService _marketReviewService;

    public MarketReviewController(IMarketReviewService marketReviewService)
    {
        _marketReviewService = marketReviewService;
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<GetMarketReviewDto>> GetMarketReview(Guid id)
    {
        return Ok(await _marketReviewService.GetByIdAsync(id));
    }

    [HttpGet("get-posts")]
    public async Task<ActionResult<IEnumerable<GetMarketReviewDto>>> GetMarketReviews([FromQuery] MarketReviewFilter marketReviewFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams)
    {
        return Ok(await _marketReviewService.GetAllAsync(marketReviewFilter, sortParams, pageParams));
    }

    [HttpPut("update-review/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMarketReview(Guid id,[FromBody] UpdateMarketReviewDto dto)
    {
        var result = await _marketReviewService.UpdateAsync(id, dto);
        
        if(!result)
            return NotFound($"Post with id {id} not found");
        
        return Ok();
    }

    [HttpPost("create-review")]
    [Authorize]
    public async Task<ActionResult<GetMarketReviewDto>> CreateMarketReview([FromBody] CreateMarketReviewDto dto)
    {
        var createdReview = await _marketReviewService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetMarketReview), new { id = createdReview.Id }, createdReview);
    }

    [HttpDelete("delete-review/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMarketReview(Guid id)
    {
        var result = await _marketReviewService.DeleteAsync(id);
        if(!result)
            return NotFound($"Post with id {id} not found");
        
        return Ok();
    }
}