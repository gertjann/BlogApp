using BlogApp.Models;

namespace BlogApp.Interfaces
{
    public interface IUnitOfWork 
    {
        IPostRepository Posts { get; }
        ICategoryRepository Categories { get; }
        IPostCategoryRepository PostCategories { get; }  

        Task CommitAsync();
    }
}
