using System.Security.Claims;
using HealthBeside.Application.Contracts.Forum.ForumCommentDto;
using HealthBeside.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumCommentController : ControllerBase
{
    private readonly IForumCommentService _forumCommentService;
    private readonly ILogger<ForumCommentController> _logger;

    public ForumCommentController(IForumCommentService forumCommentService, ILogger<ForumCommentController> logger)
    {
        _forumCommentService = forumCommentService;
        _logger = logger;
    }
    
    [HttpGet("get-comment-by-id")]
    public async Task<ActionResult<GetDetailedForumCommentDto>> GetCommentById([FromQuery] Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to get comment with ID: {CommentId}", id);
        try
        {
            var post = await _forumCommentService.GetByIdAsync(id, cancellationToken);
            _logger.LogInformation("Successfully retrieved comment with ID: {CommentId}", id);
            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving forum comment with ID {CommentId}", id);
            return NotFound("Comment not found.");
        }
    }
    
    [HttpGet("get-comments")]
    public async Task<IActionResult> GetForumComments(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to get all forum comments.");
        var comments = await _forumCommentService.GetAllAsync(cancellationToken);
        _logger.LogInformation("Successfully retrieved all forum comments.");
        return Ok(comments);
    }
    
    [HttpPost("publish-comment")]
    [Authorize]
    public async Task<ActionResult<GetForumCommentDto>> PublishComment(
        [FromBody] CreateForumCommentDto createDto, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to publish new comment for post: {PostId}", createDto.PostId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to publish comment. Missing or invalid user ID.");
            return Unauthorized("Invalid or missing user identifier.");
        }
        
        try
        {
            var createdPost = await _forumCommentService.CreateAsync(createDto, userId, cancellationToken);
            _logger.LogInformation("Forum comment {CommentId} created successfully by user {UserId}", createdPost.Id, userId);
            return CreatedAtAction(nameof(GetCommentById), new { id = createdPost.Id }, createdPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while publishing forum comment by user {UserId}", userId);
            return BadRequest("Failed to create post.");
        }
    }
    
    [HttpPut("update-comment")]
    [Authorize]
    public async Task<IActionResult> UpdateComment(
        [FromBody] UpdateForumCommentDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update comment with ID: {CommentId}", request.CommentId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to update comment. Missing or invalid user ID.");
            return Unauthorized("Invalid or missing user identifier.");
        }
        
        try
        {
            var result = await _forumCommentService.UpdateAsync(request, userId, cancellationToken);
            _logger.LogInformation("Comment with ID {CommentId} updated successfully by user {UserId}", request.CommentId, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("User {UserId} unauthorized to update comment {CommentId}", userId, request.CommentId);
            return StatusCode(403, "You are not allowed to edit this comment.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation failed while updating comment {CommentId}: {Message}", request.CommentId,
                ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating comment {CommentId} by user {UserId}", request.CommentId, userId);
            return StatusCode(500, "An error occurred while updating the comment.");
        }
    }
    

    [HttpDelete("delete-comment")]
    [Authorize]
    public async Task<IActionResult> DeleteComment([FromBody] DeleteForumCommentDto request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempt to delete forum comment {CommentId}", request.CommentId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Unauthorized attempt to delete comment. Missing or invalid user ID.");
            return Unauthorized("Invalid or missing user identifier.");
        }

        try
        {
            var result = await _forumCommentService.DeleteAsync(request.CommentId, userId, cancellationToken);

            if (!result)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found for deletion.", request.CommentId);
                return NotFound($"Comment with ID {request.CommentId} not found.");
            }

            _logger.LogInformation("Comment with ID {CommentId} deleted by user {UserId}", request.CommentId, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("User {UserId} unauthorized to delete comment {CommentId}", userId, request.CommentId);
            return StatusCode(403, "You are not authorized to delete this comment.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting comment {CommentId}", request.CommentId);
            return StatusCode(500, "An error occurred while deleting the comment.");
        }
    }
}