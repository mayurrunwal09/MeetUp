﻿namespace MeetUp.Domain.ViewModels
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string>? Roles { get; set; }
    }
}
