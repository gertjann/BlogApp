using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace BlogApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<Category> _categories;

        public CategoryRepository(BlogDbContext context)
        {
            _context = context;
            _categories = _context.Set<Category>();

        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.PostCategories)
                 .ThenInclude(pc => pc.Post)    
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null && !category.PostCategories.Any())  // Nuk lejohet fshirja nëse ka postime të lidhura
            {
                _context.Categories.Remove(category);
                //await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _categories.AnyAsync(predicate);
        }
    }

}
