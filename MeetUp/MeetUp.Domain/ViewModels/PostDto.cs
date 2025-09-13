using Microsoft.AspNetCore.Http;

namespace MeetUp.Domain.ViewModels
{
    public class PostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public IFormFile? MediaFile { get; set; }
    }
}
