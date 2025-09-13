using MeetUp.Domain.MainModels;

namespace MeetUp.Domain.ViewModels
{
    public class LikedVM
    {
        public bool? IsLiked { get; set; }
        public long? PostId { get; set; }
    }
}
