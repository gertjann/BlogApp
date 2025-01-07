using BlogApp.Data;
using BlogApp.DTO;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BlogDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(BlogDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        //  list all users
        [HttpGet("list")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _context.Users
                                    .Select(u => new
                                    {
                                        u.Id,
                                        u.Username,
                                        u.Password
                                    })
                                    .ToList();

                if (users == null || !users.Any())
                {
                    return NotFound("No users found.");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // User registration
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto registerDto)
        {
            if (registerDto == null || string.IsNullOrEmpty(registerDto.Username))
            {
                return BadRequest("Data is invalid");
            }

            // Kontrollojm nëse përdoruesi ekziston
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            // Krijo përdoruesin
            var user = new User
            {
                Username = registerDto.Username,
                Password = registerDto.Password, 
                PasswordHash = HashPassword(registerDto.Password) 
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { Message = "User registered successfully!" });
        }

        // Login and token creation
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {

            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Username or Password cannot be empty");
            }


            var user = _context.Users
               .Where(u => u.Username.ToLower().Trim() == loginDto.Username.ToLower().Trim())
               .FirstOrDefault();

            // kontrolle per userin ose passwhash 
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            // Gjenerimi i JWT token
            var token = _tokenService.GenerateToken(user.Id, user.Username ?? string.Empty);

            return Ok(new { Token = token });
        }


        // Funx ofpasswhashing
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }



        // Funx of passveryfing
        private bool VerifyPassword(string password, string storedHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }
    }
}
