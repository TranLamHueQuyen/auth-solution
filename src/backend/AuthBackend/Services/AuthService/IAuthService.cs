using AuthBackend.Models;
using System;
using System.Threading.Tasks;

namespace AuthBackend.Services
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken)> RegisterAsync(RegisterRequest request);
        Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request); // ✅ LoginRequest
        Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken);
        Task LogoutAsync(Guid userId);
        Task<User?> GetUserByIdAsync(Guid userId);
    }
}
