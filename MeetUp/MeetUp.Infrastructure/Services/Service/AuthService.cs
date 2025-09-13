using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeetUp.Infrastructure.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(
                    UserManager<AppUser> userManager,
                    SignInManager<AppUser> signInManager,
                    ILogger<AuthService> logger,
                    IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                _logger.LogWarning("Invalid registration data.");
                return IdentityResult.Failed(new IdentityError { Description = "Invalid input data." });
            }

            try
            {
                var user = new AppUser
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration.");
                return IdentityResult.Failed(new IdentityError { Description = "Something went wrong." });
            }
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                _logger.LogWarning("Invalid login data.");
                return null;
            }

            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login failed for user: {Username}", model.Username);
                    return null;
                }

                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    _logger.LogWarning("User not found.");
                    return null;
                }

                var token = await GenerateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);

                return new LoginResponseDto
                {
                    Token = token,
                    Username = user.UserName,
                    Email = user.Email ?? "",
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login.");
                return null;
            }
        }

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secret = _configuration["JwtSettings:Secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout.");
                throw;
            }
        }
    }
}