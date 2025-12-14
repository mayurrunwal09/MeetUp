using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;

namespace MeetUp.Infrastructure.Services.IService
{
    public interface IChildCommentService
    {
        Task<Comment> GetChildCommentsById(long id);
        Task<(int statusCode, string message)> AddChildComments(ChildCommentVM model, AppUser user);
        Task UpdateChildComments(long id, ChildCommentVM model, AppUser user);
        Task<(bool IsSuccess, int StatusCode, string Message)> DeleteChildComments(long id, AppUser user);
    }
}
