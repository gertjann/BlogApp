using BlogApp.Enums;

namespace BlogApp.DTO
{
    public class PostUpdateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? PublishedDate { get; set; }  
        public PostStatus Status { get; set; }
    }
}
