using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MeetUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;
        private readonly UserManager<AppUser> _userManager;

        public CommentsController(ICommentsService commentsService,UserManager<AppUser> userManager)
        {
            _commentsService = commentsService;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentsByPostId(long id)
        {
            var post = await _commentsService.GetPostCommentsById(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpPost("AddComments")]
        public async Task<IActionResult> AddComments(CommentsVM model)
        {
            var user = await _userManager.GetUserAsync(User);

            var (statusCode, message) = await _commentsService.AddComments(model, user);

            return StatusCode(statusCode, message);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePost(long id, CommentsVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(); 
            await _commentsService.UpdateComments(id, model, user);
            return Ok("Comment updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(); 
            await _commentsService.DeleteComments(id, user);
            return Ok("Comment deleted.");
        }
    }
}
