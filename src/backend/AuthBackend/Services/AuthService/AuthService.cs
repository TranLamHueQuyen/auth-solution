public class AuthService : IAuthService {
    private readonly IUserRepository _repo;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository repo, ITokenService tokenService) {
        _repo = repo;
        _tokenService = tokenService;
    }

    public async Task<(string, string)> RegisterAsync(RegisterRequest request) {
        var existing = await _repo.GetByUsernameAsync(request.Username);
        if (existing != null) throw new Exception("Username already taken");

        var user = new User {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _repo.AddAsync(user);

        return await GenerateTokens(user);
    }

    public async Task<(string, string)> LoginAsync(LoginRequest request) {
        var user = await _repo.GetByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        return await GenerateTokens(user);
    }

    public async Task<(string, string)> RefreshAsync(string refreshToken, Guid userId) {
        var user = await _repo.GetByIdAsync(userId) ?? throw new Exception("User not found");
        if (user.RefreshTokenHash == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new Exception("Refresh token expired");

        if (_tokenService.HashToken(refreshToken) != user.RefreshTokenHash)
            throw new Exception("Invalid refresh token");

        return await GenerateTokens(user);
    }

    public async Task LogoutAsync(Guid userId) {
        var user = await _repo.GetByIdAsync(userId);
        if (user != null) {
            user.RefreshTokenHash = null;
            user.RefreshTokenExpiryTime = null;
            await _repo.UpdateAsync(user);
        }
    }

    private async Task<(string, string)> GenerateTokens(User user) {
        var access = _tokenService.GenerateAccessToken(user);
        var refresh = _tokenService.GenerateRefreshToken();

        user.RefreshTokenHash = _tokenService.HashToken(refresh);
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _repo.UpdateAsync(user);
        return (access, refresh);
    }
}
