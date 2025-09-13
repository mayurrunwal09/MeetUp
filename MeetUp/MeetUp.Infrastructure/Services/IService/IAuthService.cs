using MeetUp.Domain.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MeetUp.Infrastructure.Services.Service.AuthService;

namespace MeetUp.Infrastructure.Services.IService
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto model);
        // Task<SignInResult> LoginAsync(LoginDto model);
        Task<LoginResponseDto?> LoginAsync(LoginDto model);
        Task LogoutAsync();
    }
}
