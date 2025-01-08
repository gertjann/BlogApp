using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Repositories
{
    public class PostCategoryRepository : IPostCategoryRepository
    {
        private readonly BlogDbContext _context;

        public PostCategoryRepository(BlogDbContext context)
        {
            _context = context;
        }

        // Krijo një lidhje të re midis postit dhe kategorisë
        public async Task<PostCategory> CreateAsync(PostCategory postCategory)
        {
            await _context.PostCategories.AddAsync(postCategory);
            await _context.SaveChangesAsync();
            return postCategory;
        }

        // Merr të gjitha lidhjet për një post
        public async Task<IEnumerable<PostCategory>> GetCategoriesByPostIdAsync(int postId)
        {
            return await _context.PostCategories
                .Where(pc => pc.PostId == postId)
                .ToListAsync();
        }

        // Merr të gjitha lidhjet për një kategori
        public async Task<IEnumerable<PostCategory>> GetPostsByCategoryIdAsync(int categoryId)
        {
            return await _context.PostCategories
                .Where(pc => pc.CategoryId == categoryId)
                .ToListAsync();
        }

        // Fshi lidhjen midis postit dhe kategorisë
        public async Task DeleteAsync(int postId, int categoryId)
        {
            var postCategory = await _context.PostCategories
                .FirstOrDefaultAsync(pc => pc.PostId == postId && pc.CategoryId == categoryId);

            if (postCategory != null)
            {
                _context.PostCategories.Remove(postCategory);
                await _context.SaveChangesAsync();
            }
        }

        // Fshi të gjitha lidhjet për një post
        public async Task DeleteByPostIdAsync(int postId)
        {
            var postCategories = await _context.PostCategories
                .Where(pc => pc.PostId == postId)
                .ToListAsync();

            _context.PostCategories.RemoveRange(postCategories);
            await _context.SaveChangesAsync();
        }

        // Fshi të gjitha lidhjet për një kategori
        public async Task DeleteByCategoryIdAsync(int categoryId)
        {
            var postCategories = await _context.PostCategories
                .Where(pc => pc.CategoryId == categoryId)
                .ToListAsync();

            _context.PostCategories.RemoveRange(postCategories);
            await _context.SaveChangesAsync();
        }

        public Task<bool> AnyAsync(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}
