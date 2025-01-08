using BlogApp.Data;
using BlogApp.DTO;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Services
{
    public class ValidationService : IValidationService
    {
        private readonly BlogDbContext _context;

        public ValidationService(BlogDbContext context)
        {
            _context = context;
        }

        // Validimi për kategori
        public async Task<ValidationResult> ValidateCategoryAsync(CategoryDto categoryDto)
        {
            if (string.IsNullOrEmpty(categoryDto.Name))
            {
                return new ValidationResult(false, "Category name is required.");
            }

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == categoryDto.Name);
            if (existingCategory != null)
            {
                return new ValidationResult(false, "Category already exists.");
            }

            return new ValidationResult(true);
        }

        // Validimi për postim (Create)
        public async Task<ValidationResult> ValidatePostAsync(PostCreateDto postDto)
        {
            if (string.IsNullOrEmpty(postDto.Title) || string.IsNullOrEmpty(postDto.Content))
            {
                return new ValidationResult(false, "Title and content are required.");
            }

            var categories = await _context.Categories
                .Where(c => postDto.CategoryIds.Contains(c.Id))
                .ToListAsync();
            if (!categories.Any())
            {
                return new ValidationResult(false, "Invalid categories.");
            }

            return new ValidationResult(true);
        }
        // Validimi për postim (Update)
        public async Task<ValidationResult> ValidatePostUpdateAsync(PostUpdateDto postUpdateDto)
        {
            if (string.IsNullOrEmpty(postUpdateDto.Title) || string.IsNullOrEmpty(postUpdateDto.Content))
            {
                return new ValidationResult(false, "Title and content are required.");
            }

            return new ValidationResult(true);
        }
        // Validimi për regjistrimin e përdoruesit
        public async Task<ValidationResult> ValidateRegisterAsync(UserRegisterDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.Username))
            {
                return new ValidationResult(false, "Username is required.");
            }

            if (string.IsNullOrEmpty(registerDto.Password))
            {
                return new ValidationResult(false, "Password is required.");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                return new ValidationResult(false, "Username already exists.");
            }

            return new ValidationResult(true);
        }
        // Validimi për login të përdoruesit
        public async Task<ValidationResult> ValidateLoginAsync(UserLoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Username))
            {
                return new ValidationResult(false, "Username is required.");
            }

            if (string.IsNullOrEmpty(loginDto.Password))
            {
                return new ValidationResult(false, "Password is required.");
            }

            return new ValidationResult(true);
        }


        public Task<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePostAsync(CategoryWithPostsDto categoryWithPostsDto)
        {
            throw new NotImplementedException();
        }

        public Task<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePostAsync(PostDto postDto)
        {
            throw new NotImplementedException();
        }

        public Task<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePostAsync(PostUpdateDto postUpdateDto)
        {
            throw new NotImplementedException();
        }

        public Task<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePostAsync(UserLoginDto userLoginDto)
        {
            throw new NotImplementedException();
        }

        public Task<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePostAsync(UserRegisterDto userRegister)
        {
            throw new NotImplementedException();
        }

        Task<System.ComponentModel.DataAnnotations.ValidationResult> IValidationService.ValidateCategoryAsync(CategoryDto categoryDto)
        {
            throw new NotImplementedException();
        }

        Task<System.ComponentModel.DataAnnotations.ValidationResult> IValidationService.ValidatePostAsync(PostCreateDto postDto)
        {
            throw new NotImplementedException();
        }

        public Task<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePostCreateAsync(PostCreateDto postDto)
        {
            throw new NotImplementedException();
        }

        Task<System.ComponentModel.DataAnnotations.ValidationResult> IValidationService.ValidatePostUpdateAsync(PostUpdateDto postUpdateDto)
        {
            throw new NotImplementedException();
        }

        Task<System.ComponentModel.DataAnnotations.ValidationResult> IValidationService.ValidateRegisterAsync(UserRegisterDto registerDto)
        {
            throw new NotImplementedException();
        }

        Task<System.ComponentModel.DataAnnotations.ValidationResult> IValidationService.ValidateLoginAsync(UserLoginDto loginDto)
        {
            throw new NotImplementedException();
        }
    }
}
