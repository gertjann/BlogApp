using BlogApp.Models;
using System.Linq.Expressions;

namespace BlogApp.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<Category> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> AnyAsync(Expression<Func<Category, bool>> predicate); // Për të kontrolluar ekzistencën

    }
}
