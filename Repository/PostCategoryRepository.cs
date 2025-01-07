using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
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

        // Implementimi i metodës për të kontrolluar nëse ekziston një lidhje post-kategori
        public async Task<bool> AnyAsync(Expression<Func<PostCategory, bool>> predicate)
        {
            return await _context.PostCategories.AnyAsync(predicate);
        }
    }
}
