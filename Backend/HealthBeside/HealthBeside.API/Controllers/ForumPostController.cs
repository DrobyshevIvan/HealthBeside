using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthBeside.Application.Contracts.Forum.ForumPostDto;
using HealthBeside.Application.Interfaces;
using HealthBeside.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthBeside.Domain.Models.Forum;
using HealthBeside.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace HealthBeside.API.Controllers
{
    [Route("api/[controller]")]
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

        // GET: api/ForumPost
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetForumPostDto>>> GetForumPosts()
        {
            return Ok(await _forumPostService.GetAll());
        }

        // GET: api/ForumPost/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetDetailedForumPostDto>> GetForumPost(Guid id)
        {
            return  Ok(await _forumPostService.GetById(id));
        }

        // // PUT: api/ForumPost/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutForumPost(Guid id, ForumPost forumPost)
        // {
        //     if (id != forumPost.Id)
        //     {
        //         return BadRequest();
        //     }
        //
        //     _context.Entry(forumPost).State = EntityState.Modified;
        //
        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!ForumPostExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }
        //
        //     return NoContent();
        // }

        // POST: api/ForumPost
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<GetDetailedForumPostDto>> PostForumPost(CreateForumPostDto forumPost)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId is null) 
                return Unauthorized("User ID not found in claims");
            
            if (!Guid.TryParse(userId, out var userGuid))
                return BadRequest("Invalid user ID format");

            var createdForumPost = await _forumPostService.Create(forumPost, userGuid);
            
            return CreatedAtAction("GetForumPost", new {id = createdForumPost.Id}, createdForumPost);
        }

        // DELETE: api/ForumPost/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumPost(Guid id)
        {
            var result = await _forumPostService.Delete(id);
            if (result == false)
            {
                return NotFound();
            }
        
            return NoContent();
        }
        
        private bool ForumPostExists(Guid id)
        {
            return _context.ForumPosts.Any(e => e.Id == id);
        }
    }
}
