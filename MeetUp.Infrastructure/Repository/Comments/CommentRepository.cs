using MeetUp.Domain.MainModels;
using MeetUp.Infrastructure.Context;
using MeetUp.Infrastructure.Repository.common;

namespace MeetUp.Infrastructure.Repository.Comments
{
    public class CommentRepository : Repository<Comment> , ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }
    }
}
