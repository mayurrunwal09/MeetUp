using MeetUp.Domain.MainModels;
using MeetUp.Infrastructure.Context;
using MeetUp.Infrastructure.Repository.common;

namespace MeetUp.Infrastructure.Repository.Posts
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
