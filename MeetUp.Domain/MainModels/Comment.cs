using MeetUp.Domain.BaseModel;

namespace MeetUp.Domain.MainModels
{
    public class Comment : BaseClass
    {
        public string? Text { get; set; }
        public long? PostId { get; set; }
        public string? UserId { get; set; }
        public bool? IsDeleted { get; set; }
        public Post? Post { get; set; }
        public AppUser? User { get; set; }
    }
}
