using System.Security.Claims;
using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HealthBeside.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace HealthBeside.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ForumPostController : ControllerBase
    {
        private readonly IForumPostService _forumPostService;

        public ForumPostController(IForumPostService forumPostService)
        {
            _forumPostService = forumPostService;
        }

        [HttpGet("get-posts")]
        public async Task<ActionResult<IEnumerable<GetForumPostDto>>> GetForumPosts()
        {
            return Ok(await _forumPostService.GetAllAsync());
        }

        [HttpGet("get-by-id")]
        public async Task<ActionResult<GetDetailedForumPostDto>> GetForumPost(Guid id)
        {
            return Ok(await _forumPostService.GetByIdAsync(id));
        }

        [HttpPut("update-post")]
        public async Task<IActionResult> Update([FromBody] UpdateForumPostDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid or missing user identifier.");
          
            try
            {
                var result = await _forumPostService.UpdateAsync(request, userId);

                if (result is null)
                    return NotFound("Post not found.");

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, "You are not allowed to edit this post.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("publish-post")]
        [Authorize]
        public async Task<ActionResult<GetDetailedForumPostDto>> PublishForumPost([FromBody] CreateForumPostDto createDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid or missing user identifier.");

            var createdPost = await _forumPostService.CreateAsync(createDto, userId);
            return CreatedAtAction(nameof(GetForumPost), new { id = createdPost.Id }, createdPost);
        }

        [HttpDelete("delete-post")]
        public async Task<IActionResult> DeleteForumPost(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid or missing user identifier.");
            
            var post = await _forumPostService.GetByIdAsync(id);
            
            if(post == null)
                return NotFound(new { Message = $"Post with id {id} not found." });
            
            try
            {
                var result = await _forumPostService.DeleteAsync(id, userId);

                if (!result)
                    return NotFound($"Post with id {id} not found.");

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, "You are not authorized to delete this post.");
            }
        }
        
        [HttpGet("is-post-exits")]
        private async Task<bool> ForumPostExists(Guid id)
        {
            return await _forumPostService.Exists(id);
        }
    }
}
