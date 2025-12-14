using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Repository.ChildComments;
using MeetUp.Infrastructure.Repository.Posts;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MeetUp.Infrastructure.Services.Service
{
    public class ChildCommentService : IChildCommentService
    {
        private readonly IChildCommentRepository _childCommentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<ChildCommentService> _logger;
        public ChildCommentService(IChildCommentRepository childCommentRepository, ILogger<ChildCommentService> logger,IPostRepository postRepository)
        {
            _childCommentRepository = childCommentRepository;
            _logger = logger;
            _postRepository = postRepository;
        }

        public async Task<(int statusCode, string message)> AddChildComments(ChildCommentVM model, AppUser user)
        {
            if (model != null && user != null)
                if (user == null)
                    return ((int)HttpStatusCode.Unauthorized, "User is not authorized.");

            if (string.IsNullOrWhiteSpace(model.Text))
                return ((int)HttpStatusCode.BadRequest, "Child comment text cannot be empty.");

            try
            {
                var objChildComment = new ChildComment
                {
                    ParentCommentId = model.ParentCommentId,
                    Text = model.Text.Trim(),
                    PostId = model.PostId,
                    UserId = user.Id,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = user.UserName,
                };

                await _childCommentRepository.AddAsync(objChildComment);
                await _childCommentRepository.SaveAsync();

                return ((int)HttpStatusCode.OK, "Child comment added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding comment for PostId: {PostId}", model.PostId);
                return ((int)HttpStatusCode.InternalServerError, "An error occurred while adding the comment.");
            }
        }

        public async Task<(bool IsSuccess, int StatusCode, string Message)> DeleteChildComments(long id, AppUser user)
        {
            try
            {
                var objChildComment = await _childCommentRepository.GetByIdAsync(id);

                if (objChildComment == null || objChildComment.IsDeleted == true)
                {
                    _logger.LogWarning("Delete failed: Comment with ID {Id} not found or already deleted.", id);
                    return (false, 404, "Comment not found.");
                }

                var post = await _postRepository.GetByIdAsync(objChildComment.PostId);

                if (post == null)
                {
                    _logger.LogWarning("Delete failed: Associated post not found for comment ID {Id}.", id);
                    return (false, 404, "Associated post not found.");
                }

                // Only the comment owner or the post owner can delete
                bool isCommentOwner = objChildComment.UserId == user.Id;
                bool isPostOwner = post.UserId == user.Id;

                if (!isCommentOwner && !isPostOwner)
                {
                    return (false, 403, "You are not authorized to delete this comment.");
                }

                objChildComment.IsDeleted = true;
                objChildComment.UpdatedOn = DateTime.UtcNow;
                objChildComment.UpdatedBy = user.UserName;

                await _childCommentRepository.SaveAsync();
                return (true, 200, "Child comment deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting comment with ID: {Id}", id);
                throw;
            }
        }

        public Task<Comment> GetChildCommentsById(long id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateChildComments(long id, ChildCommentVM model, AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}
