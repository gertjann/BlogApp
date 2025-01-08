using BlogApp.Data;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Interfaces
{
    public interface IPostCategoryRepository
    {
        Task<PostCategory> CreateAsync(PostCategory postCategory);
        Task<IEnumerable<PostCategory>> GetCategoriesByPostIdAsync(int postId);
        Task<IEnumerable<PostCategory>> GetPostsByCategoryIdAsync(int categoryId);
        Task DeleteAsync(int postId, int categoryId);
        Task DeleteByPostIdAsync(int postId);
        Task DeleteByCategoryIdAsync(int categoryId);
        Task<bool> AnyAsync(Func<object, bool> value);
    }
}
