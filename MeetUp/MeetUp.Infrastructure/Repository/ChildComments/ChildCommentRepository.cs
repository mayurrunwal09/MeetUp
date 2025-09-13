using MeetUp.Domain.MainModels;
using MeetUp.Infrastructure.Context;
using MeetUp.Infrastructure.Repository.common;

namespace MeetUp.Infrastructure.Repository.ChildComments
{
    public class ChildCommentRepository : Repository<ChildComment> , IChildCommentRepository
    {
        public ChildCommentRepository(ApplicationDbContext context) : base(context) { }
    }
}
