using AuthBackend.Models;

namespace AuthBackend.Services
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken)> RegisterAsync(RegisterRequest request);
        Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request);
        Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken, Guid userId);
        Task LogoutAsync(Guid userId);
    }
}
