using BlogApp.DTO;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Interfaces
{
    public interface IValidationService
    {
        Task<ValidationResult> ValidateCategoryAsync(CategoryDto categoryDto);
        Task<ValidationResult> ValidatePostAsync(PostCreateDto postDto);
        Task<ValidationResult> ValidatePostAsync(CategoryWithPostsDto categoryWithPostsDto);
        Task<ValidationResult> ValidatePostAsync(PostDto postDto);
        Task<ValidationResult> ValidatePostAsync(PostUpdateDto postUpdateDto);
        Task<ValidationResult> ValidatePostAsync(UserLoginDto userLoginDto);
        Task<ValidationResult> ValidatePostAsync(UserRegisterDto userRegister);
        Task<ValidationResult> ValidatePostCreateAsync(PostCreateDto postDto);
        Task<ValidationResult> ValidatePostUpdateAsync(PostUpdateDto postUpdateDto);
        Task<ValidationResult> ValidateRegisterAsync(UserRegisterDto registerDto); 
        Task<ValidationResult> ValidateLoginAsync(UserLoginDto loginDto); 

    }
}
