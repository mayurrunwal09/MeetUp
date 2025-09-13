using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;

namespace MeetUp.Infrastructure.Services.IService
{
    public interface ICommentsService
    {
        Task<Comment> GetPostCommentsById(long id);
        Task<(int statusCode, string message)> AddComments(CommentsVM model, AppUser user);
        Task UpdateComments(long id, CommentsVM model, AppUser user);
        Task<(bool IsSuccess, int StatusCode, string Message)> DeleteComments(long id, AppUser user);
    }
}
