using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace BlogApp.Models
{
    public class User   
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
        public string PasswordHash { get; set; } 
        public ICollection<Post> Posts { get; set; } 
    }
}
