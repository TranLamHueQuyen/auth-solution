using AuthBackend.Models;

namespace AuthBackend.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}