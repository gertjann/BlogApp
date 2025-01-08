using BlogApp.Interfaces;
using BlogApp.Models;
using BlogApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlogApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCategoryController : ControllerBase
    {
        private readonly IPostCategoryRepository _postCategoryRepository;

        public PostCategoryController(IPostCategoryRepository postCategoryRepository)
        {
            _postCategoryRepository = postCategoryRepository;
        }

        // Krijo një lidhje të re midis postit dhe kategorisë
        [HttpPost]
        public async Task<IActionResult> CreatePostCategory([FromBody] PostCategory postCategory)
        {
            var createdPostCategory = await _postCategoryRepository.CreateAsync(postCategory);
            return CreatedAtAction(nameof(GetCategoriesByPostId), new { postId = postCategory.PostId }, createdPostCategory);
        }

        // Merr të gjitha kategoritë për një post
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCategoriesByPostId(int postId)
        {
            var postCategories = await _postCategoryRepository.GetCategoriesByPostIdAsync(postId);
            return Ok(postCategories);
        }

        // Merr të gjithë postimet për një kategori
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetPostsByCategoryId(int categoryId)
        {
            var postCategories = await _postCategoryRepository.GetPostsByCategoryIdAsync(categoryId);
            return Ok(postCategories);
        }
    }
}
