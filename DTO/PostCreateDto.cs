using BlogApp.Enums;

namespace BlogApp.DTO
{
    public class PostCreateDto
    {
        public string Title { get; set; }           
        public string Content { get; set; }          
        public PostStatus Status { get; set; }   
        public DateTime? PublishedAt { get; set; }   
        public List<int> CategoryIds { get; set; }   
    }
}
