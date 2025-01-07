using BlogApp.Models;

namespace BlogApp.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> CreateAsync(Post post);
        Task<Post> GetByIdAsync(int id);
        Task<IEnumerable<Post>> GetAllAsync();
        //Task UpdateAsync(Post post);
        Task DeleteAsync(int id);
        Task<IEnumerable<Post>> FilterPostsAsync(string title, DateTime? startDate);
        Task Update(Post post);  
        Task Delete(Post post);  
        Task<bool> AnyAsync(Func<object, bool> value);
    }
}
