using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Repository.Comments;
using MeetUp.Infrastructure.Repository.Posts;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MeetUp.Infrastructure.Services.Service
{
    public class CommentsService : ICommentsService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<CommentsService> _logger;

        public CommentsService(ICommentRepository commentRepository, ILogger<CommentsService> logger, IPostRepository postRepository)
        {
            _commentRepository = commentRepository;
            _logger = logger;
            _postRepository = postRepository;
        }

        public async Task<(int statusCode, string message)> AddComments(CommentsVM model, AppUser user)
        {
            if (user == null)
                return ((int)HttpStatusCode.Unauthorized, "User is not authorized.");

            if (string.IsNullOrWhiteSpace(model.Text))
                return ((int)HttpStatusCode.BadRequest, "Comment text cannot be empty.");

            try
            {
                var comment = new Comment
                {
                    Text = model.Text.Trim(),
                    PostId = model.PostId,
                    UserId = user.Id,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = user.UserName,
                };

                await _commentRepository.AddAsync(comment);
                await _commentRepository.SaveAsync();

                return ((int)HttpStatusCode.OK, "Comment added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding comment for PostId: {PostId}", model.PostId);
                return ((int)HttpStatusCode.InternalServerError, "An error occurred while adding the comment.");
            }
        }

        public async Task<(bool IsSuccess, int StatusCode, string Message)> DeleteComments(long id, AppUser user)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(id);

                if (comment == null)
                {
                    _logger.LogWarning("Delete failed: Comment with ID {Id} not found or already deleted.", id);
                    return (false, 404, "Comment not found.");
                }

                var post = await _postRepository.GetByIdAsync(comment.PostId);

                if (post == null)
                {
                    _logger.LogWarning("Delete failed: Associated post not found for comment ID {Id}.", id);
                    return (false, 404, "Associated post not found.");
                }

                // Only the comment owner or the post owner can delete
                bool isCommentOwner = comment.UserId == user.Id;
                bool isPostOwner = post.UserId == user.Id;

                if (!isCommentOwner && !isPostOwner)
                {
                    return (false, 403, "You are not authorized to delete this comment.");
                }

                comment.IsDeleted = true;
                comment.UpdatedOn = DateTime.UtcNow;
                comment.UpdatedBy = user.UserName;

                await _commentRepository.SaveAsync();
                return (true, 200, "Comment deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting comment with ID: {Id}", id);
                throw;
            }
        }

        public Task<Comment> GetPostCommentsById(long id)
        {
            _logger.LogWarning("GetPostCommentsById is not yet implemented.");
            throw new NotImplementedException("This method is not implemented.");
        }

        public async Task UpdateComments(long id, CommentsVM model, AppUser user)
        {
            try
            {
                var commentObj = await _commentRepository.GetByIdAsync(id);
                if (commentObj == null)
                {
                    _logger.LogWarning("Update failed: Comment with ID {Id} not found.", id);
                    return;
                }

                commentObj.Text = model.Text;
                commentObj.PostId = model.PostId;
                commentObj.UserId = user.Id;
                commentObj.UpdatedOn = DateTime.UtcNow;
                commentObj.UpdatedBy = user.UserName;

                _commentRepository.Update(commentObj);
                await _commentRepository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating comment with ID: {Id}", id);
                throw;
            }
        }
    }
}