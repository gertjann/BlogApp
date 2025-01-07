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

        public CategoryController(IUnitOfWork unitOfWork,ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        // Endpoint për krijimin e kategorisë
        [Authorize(AuthenticationSchemes = "Bearer")]  // Vetëm përdoruesit e autentikuar mund të krijojnë kategori
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            // Meerr tokenin nga headeri Authorization
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // marrim user id nga funx
            var userId = _tokenService.GetUserIdFromToken(token);

            if (userId != 16)
            {
                return StatusCode(403, "Unauthorized access. userId not found.");
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            var createdCategory = await _unitOfWork.Categories.CreateAsync(category);
            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        // Endpoint për marrjen e kategorisë me ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // Endpoint për listimin e të gjitha kategorive
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return Ok(categories);
        }

        // Endpoint për përditësimin e kategorisë
        [Authorize(AuthenticationSchemes = "Bearer")]  // Vetëm përdoruesit e autentikuar mund të updatojne  kategori
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = categoryDto.Name;
            existingCategory.Description = categoryDto.Description;

            //_unitOfWork.Categories.UpdateAsync(existingCategory);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
        // Endpoint për fshirjen e kategorisë
        [Authorize(AuthenticationSchemes = "Bearer")]  // Vetëm përdoruesit e autentikuar mund të fshijne kategori
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Kontrollo nëse kategoria është e lidhur me postime
            var isLinkedToPosts = await _unitOfWork.PostCategories.AnyAsync(pc => pc.CategoryId == id);
            if (isLinkedToPosts)
            {
                return BadRequest("This category cannot be deleted because it is linked to posts.");
            }

            _unitOfWork.Categories.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
