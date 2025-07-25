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
            return Ok(await _forumPostService.GetAll());
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<ActionResult<GetDetailedForumPostDto>> GetForumPost(Guid id)
        {
            return  Ok(await _forumPostService.GetById(id));
        }

        [HttpPut("update-post/{id}")]
         public async Task<IActionResult> UpdateForumPost(Guid id, [FromBody] UpdateForumPostDto updateDto)
         {
             var success = await _forumPostService.Update(id, updateDto);

             if (!success)
             {
                 return NotFound("Post with the specified ID was not found.");
             }
    
             return Ok("Forum post updated successfully.");
         }

         //TODO Fix this method
        [HttpPost("publish-post")]
        [Authorize]
        public async Task<ActionResult<GetDetailedForumPostDto>> PublishForumPost([FromBody] CreateForumPostDto createDto)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        [HttpDelete("delete-post/{id}")]
        public async Task<IActionResult> DeleteForumPost(Guid id)
        {
            var result = await _forumPostService.Delete(id);
            if (result == false)
            {
                return NotFound();
            }
        
            return NoContent();
        }
        
        [HttpGet("is-post-exits/{id}")]
        private bool ForumPostExists(Guid id)
        {
            return _context.ForumPosts.Any(e => e.Id == id);
        }
    }
}
