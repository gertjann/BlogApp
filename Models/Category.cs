using Microsoft.Extensions.Hosting;

namespace BlogApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // public List<Post> Posts { get; set; } // Many categories -> many posts
        public ICollection<PostCategory> PostCategories { get; set; } // many to many 

    }
}
