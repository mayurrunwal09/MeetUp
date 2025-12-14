using MeetUp.Domain.MainModels;
using MeetUp.Infrastructure.Context;
using MeetUp.Infrastructure.Repository.common;

namespace MeetUp.Infrastructure.Repository.Liked
{
    public class LikeRepository :Repository<Like>, ILikeRepository
    {
        public LikeRepository(ApplicationDbContext context) : base(context) { }
    }
}
