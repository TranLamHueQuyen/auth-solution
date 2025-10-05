using AuthBackend.Models;
using AuthBackend.Repositories;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace AuthBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly ITokenService _tokenService;
        
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepo,
                           IRefreshTokenRepository refreshRepo,
                           ITokenService tokenService,
                           IUserRepository userRepository)
        {
            _userRepo = userRepo;
            _refreshRepo = refreshRepo;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        public async Task<(string accessToken, string refreshToken)> RegisterAsync(RegisterRequest request)
        {
            var existing = await _userRepo.GetByUsernameAsync(request.Username);
            if (existing != null)
                throw new InvalidOperationException("Username already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User"
            };

            await _userRepo.AddAsync(user);

            return await GenerateAndStoreTokensAsync(user);
        }

        public async Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            // Tạo access + refresh token mới
            return await GenerateAndStoreTokensAsync(user);
        }


        public async Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken)
        {
            var hashed = _tokenService.HashToken(refreshToken);
            var stored = await _refreshRepo.GetByHashAsync(hashed);

            if (stored == null)
            {
                var revoked = await _refreshRepo.GetRevokedByHashAsync(hashed);
                if (revoked != null)
                {
                    await _refreshRepo.RevokeAllForUserAsync(revoked.UserId, "Refresh token reuse detected");
                    throw new UnauthorizedAccessException("Detected refresh token reuse. Please login again.");
                }
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            if (stored.ExpiryDate < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Expired refresh token.");

            var (access, newRefresh) = await GenerateAndStoreTokensAsync(stored.User);
            var hashedNew = _tokenService.HashToken(newRefresh);
            await _refreshRepo.RevokeAsync(stored.Id, "Rotated", hashedNew);

            return (access, newRefresh);
        }

        public async Task LogoutAsync(Guid userId)
        {
            await _refreshRepo.RevokeAllForUserAsync(userId, "User logout");
        }

        private async Task<(string accessToken, string refreshToken)> GenerateAndStoreTokensAsync(User user)
        {
            var access = _tokenService.GenerateAccessToken(user);
            var refresh = _tokenService.GenerateRefreshToken();
            var hashed = _tokenService.HashToken(refresh);

            var token = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = hashed,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            await _refreshRepo.AddAsync(token);
            return (access, refresh);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

    }
}