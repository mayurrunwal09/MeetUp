using MeetUp.Domain.MainModels;

namespace MeetUp.Domain.ViewModels
{
    public class PostDetailsVM
    {
        public long PostId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public bool IsLoginUserLiked { get; set; }
        public bool? IsLoginUserPost { get; set; }
        public int? LikeCount { get; set; }
        public int? CommentCount { get; set; }
        public List<CommentVM>? Comments { get; set; }
    }
    public class CommentVM
    {
        public long CommentId { get; set; }
        public long PostId { get; set; }
        public string? Text { get; set; }
        public bool IsCommentHost { get; set; }
        public List<ChildCommentViewModel>? ChildComments { get; set; }
    }
    public class ChildCommentViewModel
    {
        public long ParentCommentId { get; set; }
        public long ChildCommentId { get; set; }
        public long PostId { get; set; }
        public string? Text { get; set; }
        public bool IsCommentHost { get; set; }
    }
}
