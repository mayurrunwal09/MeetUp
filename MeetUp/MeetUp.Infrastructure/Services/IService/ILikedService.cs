using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;

namespace MeetUp.Infrastructure.Services.IService
{
    public interface ILikedService
    {
        Task<IEnumerable<Like>> GetAllLikedPost();
        Task<Like?> GetLikedPostById(long id);
        Task AddLike(LikedVM model, AppUser user);
        Task RemoveLike(long id, LikedVM model, string userName);
    }
}
