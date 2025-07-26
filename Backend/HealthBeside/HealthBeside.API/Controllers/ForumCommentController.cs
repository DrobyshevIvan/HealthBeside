using HealthBeside.Application.Contracts.Forum.ForumCommentDto;
using HealthBeside.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumCommentController
{
    private readonly IForumCommentService _forumCommentService;

    public ForumCommentController(IForumCommentService forumCommentService)
    {
        _forumCommentService = forumCommentService;
    }
    
    [HttpPost("create-comment")]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CreateForumCommentDto request)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("update-comment")]
    [Authorize]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateForumCommentDto request)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("get-comment/{id}")]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("get-comments")]
    public async Task<IActionResult> GetAllComments()
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete-comment/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        throw new NotImplementedException();
    }
}