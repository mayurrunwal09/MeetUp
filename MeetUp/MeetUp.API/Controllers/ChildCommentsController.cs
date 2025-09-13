using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Repository.ChildComments;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MeetUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildCommentsController : ControllerBase
    {
        private readonly IChildCommentService _childCommentService;
        private readonly UserManager<AppUser> _userManager;

        public ChildCommentsController(IChildCommentService childCommentService, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _childCommentService = childCommentService;
        }

        [HttpPost("AddChildComments")]
        public async Task<IActionResult> AddChildComments(ChildCommentVM model)
        {
            var user = await _userManager.GetUserAsync(User);

            var (statusCode, message) = await _childCommentService.AddChildComments(model, user);

            return StatusCode(statusCode, message);
        }
    }
}
