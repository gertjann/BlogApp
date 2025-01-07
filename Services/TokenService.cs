using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string Username)
        {

            if (string.IsNullOrEmpty(Username))
            {
                throw new ArgumentNullException(nameof(Username), "Username cannot be null or empty");
            }

            // pas kontrolleve,vahzdojm me krijimin
            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("userId", userId.ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        public int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken != null)
            {
                var userIdClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == "userId");

                // Kontrollojm dhe kthejm një vlerë të saktë int, nëse claim-i ekziston dhe është i vlefshëm
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId; 
                }
            }
            return 0;  
        }
    }
}
