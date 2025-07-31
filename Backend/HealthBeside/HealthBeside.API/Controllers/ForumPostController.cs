using System.Security.Claims;
using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HealthBeside.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HealthBeside.API.Controllers
{
   [Route("[controller]")]
[ApiController]
public class ForumPostController : ControllerBase
{
    private readonly IForumPostService _forumPostService;
    private readonly ILogger<ForumPostController> _logger;

    public ForumPostController(IForumPostService forumPostService, ILogger<ForumPostController> logger)
    {
        _forumPostService = forumPostService;
        _logger = logger;
    }

    [HttpGet("get-posts")]
    public async Task<ActionResult<IEnumerable<GetForumPostDto>>> GetForumPosts()
    {
        _logger.LogInformation("Getting all forum posts");
        var posts = await _forumPostService.GetAllAsync();
        return Ok(posts);
    }

    [HttpGet("get-by-id")]
    public async Task<ActionResult<GetDetailedForumPostDto>> GetForumPost([FromQuery] Guid id)
    {
        _logger.LogInformation("Getting forum post with ID {PostId}", id);

        try
        {
            var post = await _forumPostService.GetByIdAsync(id);
            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving forum post with ID {PostId}", id);
            return NotFound($"Post with ID {id} not found.");
        }
    }

    [HttpPut("update-post")]
    public async Task<IActionResult> UpdateForumPost([FromBody] UpdateForumPostDto request)
    {
        _logger.LogInformation("Updating forum post with ID {PostId}", request.PostId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to update post. Missing or invalid user ID.");
            return Unauthorized("Invalid or missing user identifier.");
        }

        try
        {
            var result = await _forumPostService.UpdateAsync(request, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("User {UserId} unauthorized to update post {PostId}", userId, request.PostId);
            return StatusCode(403, "You are not allowed to edit this post.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation failed while updating post {PostId}: {Message}", request.PostId, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("publish-post")]
    [Authorize]
    public async Task<ActionResult<GetDetailedForumPostDto>> PublishForumPost([FromBody] CreateForumPostDto createDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to create post. Missing or invalid user ID.");
            return Unauthorized("Invalid or missing user identifier.");
        }

        _logger.LogInformation("User {UserId} is publishing a new forum post.", userId);

        try
        {
            var createdPost = await _forumPostService.CreateAsync(createDto, userId);
            _logger.LogInformation("Forum post {PostId} created by user {UserId}", createdPost.Id, userId);
            return CreatedAtAction(nameof(GetForumPost), new { id = createdPost.Id }, createdPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while publishing forum post by user {UserId}", userId);
            return BadRequest("Failed to create post.");
        }
    }

    [HttpDelete("delete-post")]
    public async Task<IActionResult> DeleteForumPost([FromBody] DeleteForumPostDto request)
    {
        _logger.LogInformation("Attempt to delete forum post {PostId}", request.PostId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to delete post. Missing or invalid user ID.");
            return Unauthorized("Invalid or missing user identifier.");
        }

        try
        {
            var result = await _forumPostService.DeleteAsync(request.PostId, userId);

            if (!result)
            {
                _logger.LogWarning("Post with ID {PostId} not found for deletion.", request.PostId);
                return NotFound($"Post with ID {request.PostId} not found.");
            }

            _logger.LogInformation("Post with ID {PostId} deleted by user {UserId}", request.PostId, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("User {UserId} unauthorized to delete post {PostId}", userId, request.PostId);
            return StatusCode(403, "You are not authorized to delete this post.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting post {PostId}", request.PostId);
            return StatusCode(500, "An error occurred while deleting the post.");
        }
    }
}

}
