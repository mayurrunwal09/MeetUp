using MeetUp.Domain.MainModels;
using MeetUp.Domain.ViewModels;
using MeetUp.Infrastructure.Repository.ChildComments;
using MeetUp.Infrastructure.Repository.Comments;
using MeetUp.Infrastructure.Repository.Liked;
using MeetUp.Infrastructure.Repository.Posts;
using MeetUp.Infrastructure.Services.IService;

namespace MeetUp.Infrastructure.Services.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IChildCommentRepository _childCommentRepository;
        public PostService(IPostRepository postRepository,ILikeRepository likeRepository,ICommentRepository commentRepository, IChildCommentRepository childCommentRepository)
        {
            _postRepository = postRepository;
            _likeRepository= likeRepository;
            _commentRepository= commentRepository;
            _childCommentRepository = childCommentRepository;
        }

        public async Task CreatePostAsync(PostDto dto, AppUser user)
        {
            string? mediaUrl = null;
            if (dto.MediaFile != null && dto.MediaFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.MediaFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.MediaFile.CopyToAsync(stream);
                }

                mediaUrl = "/uploads/" + uniqueFileName; 
            }
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                MediaUrl = mediaUrl,
                IsActive = true,
                UserId = user.Id,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = user.UserName,
            };

            await _postRepository.AddAsync(post);
            await _postRepository.SaveAsync();
        }

        public async Task DeletePostAsync(long id, string userName)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return;
            // Delete the image file if it exists
            if (!string.IsNullOrEmpty(post.MediaUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.MediaUrl.TrimStart('/'));

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            post.IsActive = false;
            post.UpdatedOn = DateTime.UtcNow;
            post.UpdatedBy = userName;
            await _postRepository.SaveAsync();
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            var posts = await _postRepository.GetAllAsync();
            return posts.Where(p => p.IsActive == true);
        }

        public async Task<Post?> GetPostByIdAsync(long id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            return (post != null && post.IsActive == true) ? post : null;
        }

        public async Task UpdatePostAsync(long id, PostDto dto, string userName)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return;

            // Handle new file upload (replace old file if new is uploaded)
            if (dto.MediaFile != null && dto.MediaFile.Length > 0)
            {
                // Optional: delete the old image file if it exists
                if (!string.IsNullOrEmpty(post.MediaUrl))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.MediaUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save the new image
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.MediaFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.MediaFile.CopyToAsync(stream);
                }

                post.MediaUrl = "/uploads/" + uniqueFileName;
            }

            post.Title = dto.Title;
            post.Content = dto.Content;
            post.UpdatedOn = DateTime.UtcNow;
            post.UpdatedBy = userName;

            _postRepository.Update(post);
            await _postRepository.SaveAsync();
        }

        public async Task<List<PostDetailsVM>> GetAllActivePostDetailsAsync(AppUser user)
        {
            var posts = await _postRepository.GetAllAsync(); 
            var activePosts = posts.Where(p => p.IsActive == true).OrderByDescending(p => p.Id).ToList();

            var result = new List<PostDetailsVM>();

            foreach (var post in activePosts)
            {
                var likes = await _likeRepository.GetAllAsync();

                var comments = await _commentRepository.GetAllAsync();

                var childComments = await _childCommentRepository.GetAllAsync();

                var postLikes = likes.Where(l => l.PostId == post.Id && l.IsLiked == true).ToList();

                var postComments = comments
                    .Where(c => c.PostId == post.Id && c.IsDeleted != true)
                    .ToList();

                //var postChildComments = childComments.Where(cc => cc.ParentCommentId == comments.Id && cc.PostId == post.Id).ToList();

                var isUserLiked = postLikes.Any(l => l.UserId == user.Id);

                var isLoginUserPost = post.UserId == user.Id;

                var postDetails = new PostDetailsVM
                {
                    PostId = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    MediaUrl = post.MediaUrl,

                    LikeCount = postLikes.Count,
                    CommentCount = postComments.Count,
                    Comments = postComments.Select(c => new CommentVM
                    {
                        CommentId = c.Id,
                        PostId = post.Id,
                        Text = c.Text,
                        IsCommentHost = c.UserId == user.Id,
                        ChildComments = childComments
                        .Where(cc => cc.ParentCommentId == c.Id && cc.PostId == post.Id && cc.IsDeleted != true)
                        .Select(cc => new ChildCommentViewModel
                        {
                            ParentCommentId = c.Id,
                            PostId = post.Id,
                            ChildCommentId = cc.Id,
                            Text = cc.Text,
                            IsCommentHost = cc.UserId == user.Id
                        }).ToList(),
                    }).ToList(),
                    IsLoginUserLiked = isUserLiked,
                    IsLoginUserPost = isLoginUserPost,
                };

                result.Add(postDetails);
            }
            return result;
        }

        public async Task<List<PostDetailsVM>> GetLoginUserPostsAsync(AppUser user)
        {
            var allPosts = await _postRepository.GetAllAsync();
            var userPosts = allPosts
                .Where(p => p.IsActive == true && p.UserId == user.Id).OrderByDescending(p => p.Id) 
                .ToList();

            var result = new List<PostDetailsVM>();

            foreach (var post in userPosts)
            {
                var likes = await _likeRepository.GetAllAsync();

                var comments = await _commentRepository.GetAllAsync();

                var childComments = await _childCommentRepository.GetAllAsync();

                var postLikes = likes.Where(l => l.PostId == post.Id && l.IsLiked == true).ToList();

                var postComments = comments
                    .Where(c => c.PostId == post.Id && c.IsDeleted != true)
                    .ToList();

                var isUserLiked = postLikes.Any(l => l.UserId == user.Id);

                var isLoginUserPost = post.UserId == user.Id;

                var postDetails = new PostDetailsVM
                {
                    PostId = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    MediaUrl = post.MediaUrl,

                    LikeCount = postLikes.Count,
                    CommentCount = postComments.Count,
                    Comments = postComments.Select(c => new CommentVM
                    {
                        CommentId = c.Id,
                        PostId = post.Id,
                        Text = c.Text,
                        IsCommentHost = c.UserId == user.Id,
                        ChildComments = childComments
                        .Where(cc => cc.ParentCommentId == c.Id && cc.PostId == post.Id)
                        .Select(cc => new ChildCommentViewModel
                        {
                            ParentCommentId = c.Id,
                            PostId = post.Id,
                            ChildCommentId = cc.Id,
                            Text = cc.Text,
                            IsCommentHost = cc.UserId == user.Id
                        }).ToList(),
                    }).ToList(),
                    IsLoginUserLiked = isUserLiked,
                    IsLoginUserPost = isLoginUserPost,
                };

                result.Add(postDetails);
            }
            return result;
        }

    }
}
