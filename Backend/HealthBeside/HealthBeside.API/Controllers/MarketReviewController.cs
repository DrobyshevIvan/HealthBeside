using System.Security.Claims;
using HealthBeside.Application.Contracts.MarketPlace.MarketReviewDto;
using HealthBeside.Application.Filters;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Pagination;
using HealthBeside.Application.Sorting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarketReviewController : ControllerBase
{
    private readonly IMarketReviewService _marketReviewService;
    private readonly ILogger<MarketReviewController> _logger; 

    public MarketReviewController(IMarketReviewService marketReviewService, ILogger<MarketReviewController> logger) 
    {
        _marketReviewService = marketReviewService;
        _logger = logger; 
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<GetMarketReviewDto>> GetMarketReview(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to get market review with ID: {ReviewId}", id);
        try
        {
            var review = await _marketReviewService.GetByIdAsync(id, cancellationToken);
            _logger.LogInformation("Successfully retrieved market review with ID: {ReviewId}", id); 
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving market review with ID {ReviewId}", id); 
            return NotFound("Review not found: " + ex.Message);
        }
    }

    [HttpGet("get-reviews")]
    public async Task<ActionResult<IEnumerable<GetMarketReviewDto>>> GetMarketReviews([FromQuery] MarketReviewFilter marketReviewFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to get all market reviews with filters: {Filter}, sort: {Sort}, page: {Page}", 
            marketReviewFilter?.ToString(), sortParams?.ToString(), pageParams?.ToString());

        var reviews = await _marketReviewService.GetAllAsync(marketReviewFilter, sortParams, pageParams, cancellationToken);
        
        _logger.LogInformation("Successfully retrieved {Count} market reviews.", reviews.Count()); 
        return Ok(reviews);
    }

    [HttpPut("update-review/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GetMarketReviewDto>> UpdateMarketReview(
        [FromBody] UpdateMarketReviewDto request, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update market review with ID: {ReviewId}", request.ReviewId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to update review {ReviewId}. Missing or invalid user ID.", request.ReviewId); 
            return Unauthorized("Invalid or missing user identifier.");
        }
        
        try
        {
            var result = await _marketReviewService.UpdateAsync(request.ReviewId, userId, request, cancellationToken);
            _logger.LogInformation("Market review with ID {ReviewId} updated successfully by user {UserId}", request.ReviewId, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "User {UserId} unauthorized to update review {ReviewId}", userId, request.ReviewId); 
            return StatusCode(403, "You are not allowed to edit this review.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation failed while updating review {ReviewId}: {Message}", request.ReviewId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating market review {ReviewId} by user {UserId}", request.ReviewId, userId); 
            return StatusCode(500, "An error occurred while updating the review.");
        }
    }

    [HttpPost("create-review")]
    [Authorize]
    public async Task<ActionResult<GetMarketReviewDto>> CreateMarketReview([FromBody] CreateMarketReviewDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create a new market review for ProductId: {ProductId}", dto.ProductId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to create review for ProductId {ProductId}. Missing or invalid user ID.", dto.ProductId); 
            return Unauthorized("Invalid or missing user identifier.");
        }
        
        try
        {
            var createdReview = await _marketReviewService.CreateAsync(dto, userId, cancellationToken);
            _logger.LogInformation("Market review {ReviewId} created successfully by user {UserId} for ProductId {ProductId}", createdReview.Id, userId, dto.ProductId); // Лог успіху
            return CreatedAtAction(nameof(GetMarketReview), new { id = createdReview.Id }, createdReview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating market review for ProductId {ProductId} by user {UserId}", dto.ProductId, userId); 
            return BadRequest("Failed to create review.");
        }
    }

    [HttpDelete("delete-review/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMarketReview([FromBody] DeleteMarketReviewDto request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempt to delete market review with ID: {ReviewId}", request.ReviewId); 

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to delete review {ReviewId}. Missing or invalid user ID.", request.ReviewId);
            return Unauthorized("Invalid or missing user identifier.");
        }
        
        try
        {
            var result = await _marketReviewService.DeleteAsync(request.ReviewId, userId, cancellationToken);

            if (!result)
            {
                _logger.LogWarning("Market review with ID {ReviewId} not found for deletion.", request.ReviewId);
                return NotFound($"Comment with ID {request.ReviewId} not found.");
            }

            _logger.LogInformation("Market review with ID {ReviewId} deleted successfully by user {UserId}", request.ReviewId, userId); 
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "User {UserId} unauthorized to delete review {ReviewId}", userId, request.ReviewId); 
            return StatusCode(403, "You are not authorized to delete this review.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting market review {ReviewId}", request.ReviewId); 
            return StatusCode(500, "An error occurred while deleting the review.");
        }
    }
}