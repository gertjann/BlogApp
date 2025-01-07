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

        public PostController(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        // Endpoint për krijimin e postimit
        [Authorize(AuthenticationSchemes = "Bearer")]  // Vetëm përdoruesit e autentikuar mund të krijojnë postime
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateDto postDto)
        {
            // Meerr tokenin nga headeri Authorization
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // marrim user id nga funx
            var userId = _tokenService.GetUserIdFromToken(token);
            // Merr ID-në e përdoruesit nga tokeni
            if (userId == 0)
            {
                return Unauthorized(new { Message = "Token is invalid or does not contain a valid 'userId'." });
            }

            // find categories
            var categories = await _unitOfWork.Categories.GetAllAsync();

            if (categories == null || !categories.Any())
            {
                return BadRequest("Invalid category IDs.");
            }
            var postCategories = categories.Where(c => postDto.CategoryIds.Contains(c.Id))
                                    .Select(c => new PostCategory { CategoryId = c.Id })
                                    .ToList();

            // create post
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

            return CreatedAtAction(nameof(GetPostById), new { id = createdPost.Id }, createdPost);
        }

        // Endpoint për marrjen e postimit me ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _unitOfWork.Posts.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        // List all posts 
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _unitOfWork.Posts.GetAllAsync();
            return Ok(posts);
        }

        // Endpoint për filtrimin e  postimeve
        [HttpGet("filter")]
        public async Task<IActionResult> FilterPosts([FromQuery] string? title, [FromQuery] DateTime? startDate)
        {
            var posts = await _unitOfWork.Posts.FilterPostsAsync(title, startDate);

            if (posts == null || !posts.Any())
            {
                return NotFound("No posts found with the given criteria.");
            }

            return Ok(posts);
        }

        // Endpoint për përditësimin e postimit
        [Authorize(AuthenticationSchemes = "Bearer")]  // Vetëm përdoruesit e autentikuar mund të krijojnë postime
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateDto postDto)
        {
            var post = await _unitOfWork.Posts.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            // Merr ID-në e përdoruesit nga tokeni
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = _tokenService.GetUserIdFromToken(token);

            // Kontrollojm nqs përdoruesi është ai që e ka krijuar postimin
            if (post.UserId != userId)
            {
                return Forbid("You can only edit your own posts.");
            }

            //Nqs po,e updatojme 
            post.Title = postDto.Title;
            post.Content = postDto.Content;
            post.Status = postDto.Status;

            // ruajme ndryshimet 
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]  // Vetëm përdoruesit e autentikuar mund të krijojnë postime
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _unitOfWork.Posts.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }
            // Marrim id e userit nga tokeni 
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = _tokenService.GetUserIdFromToken(token);


            // Kontrollo nëse përdoruesi është ai që e ka krijuar postimin
            if (post.UserId != userId)
            {
                return Forbid("You can only delete your own posts.");
            }

            // Nqs po,fshijme  postimin
            await _unitOfWork.Posts.Delete(post);

            return NoContent();
        }

    }
}

