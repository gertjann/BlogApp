using BlogApp.Data;
using BlogApp.DTO;
using BlogApp.Interfaces;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IValidationService _validationService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(BlogDbContext context, ITokenService tokenService, IValidationService validationService, ILogger<AuthController> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _validationService = validationService;
            _logger = logger;
        }

        // List all users
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
                    _logger.LogWarning("No users found in the database.");
                    return NotFound("No users found.");
                }
                _logger.LogInformation("Successfully retrieved the list of users.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, new { Message = "An error occurred while retrieving users.", Error = ex.Message });
            }
        }

        // User registration
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto registerDto)
        {
            try
            {
                if (registerDto == null || string.IsNullOrEmpty(registerDto.Username))
                {
                    _logger.LogWarning("Received invalid data during user registration.");
                    return BadRequest("Data is invalid.");
                }

                // Check if user already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == registerDto.Username);
                if (existingUser != null)
                {
                    _logger.LogWarning("Username already exists: {Username}", registerDto.Username);
                    return BadRequest("Username already exists.");
                }

                // Create user
                var user = new User
                {
                    Username = registerDto.Username,
                    Password = registerDto.Password,
                    PasswordHash = HashPassword(registerDto.Password)
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                _logger.LogInformation("User registered successfully: {Username}", registerDto.Username);
                return Ok(new { Message = "User registered successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, new { Message = "An error occurred during user registration.", Error = ex.Message });
            }
        }

        // Login and token creation
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                {
                    _logger.LogWarning("Login attempt with invalid credentials.");
                    return BadRequest("Username or Password cannot be empty.");
                }

                var user = _context.Users
                    .Where(u => u.Username.ToLower().Trim() == loginDto.Username.ToLower().Trim())
                    .FirstOrDefault();

                // Check if user exists and verify password
                if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for user: {Username}", loginDto.Username);
                    return Unauthorized("Invalid credentials.");
                }

                // Generate JWT token
                var token = _tokenService.GenerateToken(user.Id, user.Username ?? string.Empty);
                _logger.LogInformation("User logged in successfully: {Username}", loginDto.Username);


                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, new { Message = "An error occurred during login.", Error = ex.Message });
            }
        }

        // Function for password hashing
        private string HashPassword(string password)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return Convert.ToBase64String(hashedBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw new Exception("An error occurred while hashing the password.", ex);
            }
        }

        // Function for password verification
        private bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var hashedPassword = HashPassword(password);
                return hashedPassword == storedHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                throw new Exception("An error occurred while verifying the password.", ex);
            }
        }
    }
}
