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
        private readonly AppDbContext _context;
        private readonly IForumPostService _forumPostService;

        public ForumPostController(AppDbContext context,
            IForumPostService forumPostService)
        {
            _context = context;
            _forumPostService = forumPostService;
        }

        [HttpGet("get-posts")]
        public async Task<ActionResult<IEnumerable<GetForumPostDto>>> GetForumPosts()
        {
            return Ok(await _forumPostService.GetAllAsync());
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<ActionResult<GetDetailedForumPostDto>> GetForumPost(Guid id)
        {
            return  Ok(await _forumPostService.GetByIdAsync(id));
        }

        [HttpPut("update-post/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateForumPostDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid or missing user identifier.");

            var result = await _forumPostService.UpdateAsync(id, dto);

            if (!result)
                return NotFound($"Post with id {id} not found.");

            return NoContent();
        }

         //TODO Fix this method
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

        [HttpDelete("delete-post/{id}")]
        public async Task<IActionResult> DeleteForumPost(Guid id)
        {
            var result = await _forumPostService.DeleteAsync(id);
            if (result == false)
            {
                return NotFound();
            }
        
            return NoContent();
        }
        
        [HttpGet("is-post-exits/{id}")]
        private async Task<bool> ForumPostExists(Guid id)
        {
            return await _forumPostService.Exists(id);
        }
    }
}
