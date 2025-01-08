using BlogApp.DTO;
using BlogApp.Interfaces;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IValidationService _validationService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IUnitOfWork unitOfWork, ITokenService tokenService, ILogger<CategoryController> logger, IValidationService validationService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _logger = logger;
            _validationService = validationService;
        }

        // Endpoint për krijimin e kategorisë
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            try
            {
                string authorizationHeader = HttpContext.Request.Headers["Authorization"];
                var token = authorizationHeader.Replace("Bearer ", "");

                var userId = _tokenService.GetUserIdFromToken(token);

                var category = new Category
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description
                };

                var createdCategory = await _unitOfWork.Categories.CreateAsync(category);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Category created successfully: {CategoryName} by user {UserId}", category.Name, userId);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the category.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint për marrjen e kategorisë me ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", id);
                    return NotFound("Category not found.");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the category by ID.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint për listimin e të gjitha kategorive
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Fetching all categories.");
                var categories = await _unitOfWork.Categories.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching categories.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint për përditësimin e kategorisë
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found.");
                    return NotFound("Category not found.");
                }

                existingCategory.Name = categoryDto.Name;
                existingCategory.Description = categoryDto.Description;

                //_unitOfWork.Categories.UpdateAsync(existingCategory);
                await _unitOfWork.CommitAsync();
                _logger.LogInformation($"Category with ID {id} updated successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating category.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint për fshirjen e kategorisë
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID {id} not found.");
                    return NotFound("Category not found.");
                }

                /*var isLinkedToPosts = await _unitOfWork.PostCategories.AnyAsync(pc => pc.CategoryId == id);
                if (isLinkedToPosts)
                {
                    _logger.LogWarning($"Category with ID {id} cannot be deleted because it is linked to posts.");
                    return BadRequest("This category cannot be deleted because it is linked to posts.");
                }
                */

                _unitOfWork.Categories.DeleteAsync(id);
                await _unitOfWork.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting category.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
