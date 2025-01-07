using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly BlogDbContext _context;

    public TestController(BlogDbContext context)
    {
        _context = context;
    }

    // Test ADD Categories
    [HttpPost("add-category")]
    public async Task<IActionResult> AddCategory()
    {
        var category = new Category
        {
            Name = "Technology",
            Description = "Posts related to technology"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok("Category added successfully");
    }

    // Test GEt 
    [HttpGet("get-categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return Ok(categories);
    }
}
