using BlogApp.Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlogApp.Interfaces
{
    public interface IPostCategoryRepository
    {
        Task<bool> AnyAsync(Expression<Func<PostCategory, bool>> predicate);  // Kontrollojm nqs ekziston  lidhje post-kategori
    }
}
