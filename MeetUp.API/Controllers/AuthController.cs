using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var response = await _authService.LoginAsync(model);
            if (response == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(response); 
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok("Logged out successfully");
        }
    }
}

