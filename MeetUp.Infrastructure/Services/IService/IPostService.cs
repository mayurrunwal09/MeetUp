using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;

namespace MeetUp.Infrastructure.Services.IService
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post?> GetPostByIdAsync(long id);
        Task CreatePostAsync(PostDto dto, AppUser user);
        Task UpdatePostAsync(long id, PostDto dto, string userName);
        Task DeletePostAsync(long id,string userName);
        Task<List<PostDetailsVM>> GetAllActivePostDetailsAsync(AppUser user);
        Task<List<PostDetailsVM>> GetLoginUserPostsAsync(AppUser user);
    }
}
