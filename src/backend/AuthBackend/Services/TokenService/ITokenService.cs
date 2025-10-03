public interface ITokenService {
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashToken(string token);
}
