using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MeetUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly UserManager<AppUser> _userManager;
        public PostController(IPostService postService,UserManager<AppUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm] PostDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            await _postService.CreatePostAsync(dto, user);
            return Ok("Post created successfully.");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePost(long id, [FromForm] PostDto dto)
        {
            var userName = User.Identity?.Name ?? "Anonymous";
            await _postService.UpdatePostAsync(id, dto, userName);
            return Ok("Post updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userName = User.Identity?.Name ?? "Anonymous";
            await _postService.DeletePostAsync(id, userName);
            return Ok("Post deleted.");
        }

        [HttpGet("ActivePostDetails")]
        public async Task<IActionResult> GetActivePostDetails()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();
                var postDetails = await _postService.GetAllActivePostDetailsAsync(user);
                return Ok(postDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching post details.", error = ex.Message });
            }
        }

        [HttpGet("MyPosts")]
        public async Task<IActionResult> GetLoginUserPosts()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized(new { message = "User is not authorized." });

                var posts = await _postService.GetLoginUserPostsAsync(user);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching user posts.", error = ex.Message });
            }
        }

    }
}
