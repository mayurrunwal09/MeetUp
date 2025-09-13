using MeetUp.Domain.MainModels;

namespace MeetUp.Domain.ViewModels
{
    public class ChildCommentVM
    {
        public long? ParentCommentId { get; set; }
        public string? Text { get; set; }
        public long? PostId { get; set; }
    }
}
