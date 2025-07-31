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

    public ForumCommentController(IForumCommentService forumCommentService)
    {
        _forumCommentService = forumCommentService;
    }
    
    [HttpGet("get-comment/{id}")]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        var comment = await _forumCommentService.GetByIdAsync(id);
     
        if (comment == null)
        {
            return NotFound(new { Message = "Comment not found." });
        }
        
        return Ok(comment);
    }
    
    [HttpGet("get-comments")]
    public async Task<IActionResult> GetAllComments()
    {
        return await _forumCommentService.GetAllAsync()
            .ContinueWith(task => Ok(task.Result));
    }
    
    [HttpPut("update-comment")]
    [Authorize]
    public async Task<IActionResult> UpdateComment([FromBody] UpdateForumCommentDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Invalid or missing user identifier.");
        
        var result = await _forumCommentService.UpdateAsync(request);
        return result ? Ok("Comment updated successfully") : NotFound();
    }
    
    [HttpPost("create-comment")]
    [Authorize]
    public async Task<ActionResult<GetForumCommentDto>> CreateComment([FromBody] CreateForumCommentDto createDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Invalid or missing user identifier.");
        
        var createdComment = await _forumCommentService.CreateAsync(createDto, userId);
        return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
    }

    [HttpDelete("delete-comment/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var result = await _forumCommentService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new { Message = $"Comment with id {id} not found." });
        }
        
        return Ok($"Comment with id: {id} deleted successfully.");
    }
}