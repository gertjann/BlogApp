using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogDbContext _context;

        public PostRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<Post> CreateAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _context.Posts.Include(p => p.PostCategories)
                                       .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts.Include(p => p.PostCategories).ToListAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public Task<bool> AnyAsync(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Post>> FilterPostsAsync(string title, DateTime? startDate)
        {
            var query = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(p => p.Title.Contains(title));
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.CreatedDate >= startDate.Value);
            }

            return await query.ToListAsync();
        }
        public async Task Update(Post post)
        {
            _context.Posts.Update(post);  
            await _context.SaveChangesAsync(); 
        }
        public async Task Delete(Post post)
        {
            _context.Posts.Remove(post); 
            await _context.SaveChangesAsync(); 
        }

    }

}
