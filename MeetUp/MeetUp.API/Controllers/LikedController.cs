using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MeetUp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikedController : ControllerBase
    {
        private readonly ILikedService _iLikedService;
        private readonly UserManager<AppUser> _userManager;
        public LikedController(ILikedService service,UserManager<AppUser> userManager)
        {
            _iLikedService = service;
            _userManager = userManager;
        }

        [HttpPost("LikePost")]
        public async Task<IActionResult> AddLike(LikedVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            await _iLikedService.AddLike(model, user);
            return Ok("Like updated successfully.");
        }
    }
}
