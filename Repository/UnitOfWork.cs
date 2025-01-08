using BlogApp.Data;
using BlogApp.Interfaces;
using BlogApp.Models;
using BlogApp.Repositories;


namespace BlogApp.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogDbContext _context;

        public IPostRepository Posts { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IPostCategoryRepository PostCategories { get; private set; }

        public UnitOfWork(BlogDbContext context, IPostCategoryRepository postCategoryRepository)
        {
            _context = context;
            Posts = new PostRepository(_context); // Initialization of PostRepository
            Categories = new CategoryRepository(_context); // Initialization of CategoryRepository
            PostCategories = new PostCategoryRepository(_context); // Initialization of CategoryRepository
            //PostCategories = new PostCategoryRepository;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
