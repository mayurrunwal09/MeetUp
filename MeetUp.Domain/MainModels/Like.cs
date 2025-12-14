using MeetUp.Domain.BaseModel;

namespace MeetUp.Domain.MainModels
{
    public class Like : BaseClass
    {
        public bool? IsLiked { get; set; }
        public long? PostId { get; set; }
        public Post? Post { get; set; }

        public string? UserId { get; set; }
        public AppUser? User { get; set; }
    }
}
