using MeetUp.Domain.BaseModel;

namespace MeetUp.Domain.MainModels
{
    public class Post : BaseClass
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public bool? IsActive { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }

        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
    }
}
