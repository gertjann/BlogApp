using BlogApp.DTO;
using BlogApp.Enums;
using BlogApp.Interfaces;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ILogger<PostController> _logger;


        public PostController(IUnitOfWork unitOfWork, ITokenService tokenService, ILogger<PostController> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _logger = logger;
        }

        // Endpoint për krijimin e postimit
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateDto postDto)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = _tokenService.GetUserIdFromToken(token);

                if (userId == 0)
                {
                    _logger.LogWarning("Invalid token.");
                    return Unauthorized(new { Message = "Token is invalid or does not contain a valid 'userId'." });
                }

                var categories = await _unitOfWork.Categories.GetAllAsync();

                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found.");
                    return BadRequest("Invalid category IDs.");
                }

                var postCategories = categories.Where(c => postDto.CategoryIds.Contains(c.Id))
                                               .Select(c => new PostCategory { CategoryId = c.Id })
                                               .ToList();

                var post = new Post
                {
                    Title = postDto.Title,
                    Content = postDto.Content,
                    Status = postDto.Status,
                    CreatedDate = DateTime.Now,
                    UserId = userId,
                    PostCategories = postCategories
                };

                var createdPost = await _unitOfWork.Posts.CreateAsync(post);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation($"Post with title {postDto.Title} created successfully.");
                return CreatedAtAction(nameof(GetPostById), new { id = createdPost.Id }, createdPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint për marrjen e postimit me ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            try
            {
                var post = await _unitOfWork.Posts.GetByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning($"Post with ID {id} not found.");
                    return NotFound();
                }

                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // List all posts
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _unitOfWork.Posts.GetAllAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint për filtrimin e postimeve
        [HttpGet("filter")]
        public async Task<IActionResult> FilterPosts([FromQuery] string? title, [FromQuery] DateTime? startDate)
        {
            try
            {
                var posts = await _unitOfWork.Posts.FilterPostsAsync(title, startDate);

                if (posts == null || !posts.Any())
                {
                    _logger.LogWarning("No posts found with the given criteria.");
                    return NotFound("No posts found with the given criteria.");
                }

                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint për përditësimin e postimit
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateDto postDto)
        {
            try
            {
                var post = await _unitOfWork.Posts.GetByIdAsync(id);

                if (post == null)
                {
                    return NotFound();
                }

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = _tokenService.GetUserIdFromToken(token);

                if (post.UserId != userId)
                {
                    _logger.LogWarning($"User {userId} tried to update a post not owned by them.");
                    return Forbid("You can only edit your own posts.");
                }

                post.Title = postDto.Title;
                post.Content = postDto.Content;
                post.Status = postDto.Status;

                await _unitOfWork.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint për fshirjen e postimit
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var post = await _unitOfWork.Posts.GetByIdAsync(id);

                if (post == null)
                {
                    return NotFound();
                }

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userId = _tokenService.GetUserIdFromToken(token);

                if (post.UserId != userId)
                {
                    _logger.LogWarning($"User {userId} tried to delete a post not owned by them.");
                    return Forbid("You can only delete your own posts.");
                }

                await _unitOfWork.Posts.Delete(post);
                await _unitOfWork.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
