using Microsoft.AspNetCore.Identity;

namespace MeetUp.Domain.MainModels
{
    public class AppUser : IdentityUser
    {
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
    }
}
