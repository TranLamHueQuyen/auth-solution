using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AuthBackend.Models;
using AuthBackend.Repositories;
using AuthBackend.Services;   

namespace AuthBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        // Lưu refresh tokens tạm trong RAM (demo) → thực tế phải lưu DB/Redis
        private static readonly ConcurrentDictionary<Guid, string> _refreshTokens = new();

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<(string accessToken, string refreshToken)> RegisterAsync(RegisterRequest request)
        {
            // Check user tồn tại
            var existing = await _userRepository.GetByUsernameAsync(request.Username);
            if (existing != null)
                throw new InvalidOperationException("Username already exists.");

            // Tạo user mới
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Password  = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User"
            };

            await _userRepository.AddAsync(user);

            return GenerateTokens(user);
        }

        public async Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password ))
                throw new UnauthorizedAccessException("Invalid username or password.");

            return GenerateTokens(user);
        }

        public Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken, Guid userId)
        {
            if (string.IsNullOrEmpty(refreshToken) || !_refreshTokens.TryGetValue(userId, out var stored) || stored != refreshToken)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            // Lấy user từ repository
            var user = _userRepository.GetByIdAsync(userId).Result;
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            return Task.FromResult(GenerateTokens(user));
        }

        public Task LogoutAsync(Guid userId)
        {
            _refreshTokens.TryRemove(userId, out _);
            return Task.CompletedTask;
        }

        private (string accessToken, string refreshToken) GenerateTokens(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _refreshTokens[user.Id] = refreshToken;

            return (accessToken, refreshToken);
        }
    }
}
