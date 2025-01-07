namespace BlogApp.Services
{
    public interface ITokenService
    {
        string GenerateToken(int userId, string Username);
        int GetUserIdFromToken(string token);
    }
}
