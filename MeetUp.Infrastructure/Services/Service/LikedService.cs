using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Repository.Liked;
using MeetUp.Infrastructure.Services.IService;
using Microsoft.Extensions.Logging;

namespace MeetUp.Infrastructure.Services.Service
{
    public class LikedService : ILikedService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly ILogger<LikedService> _logger;
        public LikedService(ILikeRepository likeRepository, ILogger<LikedService> logger)
        {
            _likeRepository = likeRepository;
            _logger = logger;
        }

        public async Task AddLike(LikedVM model, AppUser user)
        {
            try
            {
                var existingLike = (await _likeRepository
                                        .FindAsync(l => l.PostId == model.PostId && l.UserId == user.Id))
                                        .FirstOrDefault();

                if (existingLike != null)
                {
                    existingLike.IsLiked = model.IsLiked;
                    existingLike.UpdatedOn = DateTime.UtcNow;
                    existingLike.UpdatedBy = user.UserName;

                    _likeRepository.Update(existingLike);
                }
                else
                {
                    var likeObj = new Like
                    {
                        IsLiked = model.IsLiked,
                        PostId = model.PostId,
                        UserId = user.Id,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = user.UserName,
                    };

                    await _likeRepository.AddAsync(likeObj);
                }

                await _likeRepository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding/updating like to PostId: {PostId}, UserId: {UserId}", model.PostId, user.Id);
                throw;
            }
        }

        public Task<IEnumerable<Like>> GetAllLikedPost()
        {
            throw new NotImplementedException();
        }

        public Task<Like?> GetLikedPostById(long id)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveLike(long id, LikedVM model, string userName)
        {
            throw new NotImplementedException();
        }
    }
}
