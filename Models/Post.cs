using BlogApp.Enums;
using System.Text.Json.Serialization;

namespace BlogApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public PostStatus Status { get; set; }  
        public DateTime CreatedDate { get; set; }
        public DateTime? PublishedDate { get; set; }

        // Connections
        public int UserId { get; set; }
        public User User { get; set; }
        //public List<Category> Categories { get; set; }
        [JsonIgnore]
        public ICollection<PostCategory> PostCategories { get; set; } //many to many 

    }
}
