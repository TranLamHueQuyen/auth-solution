using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AuthBackend.Models;
using AuthBackend.Repositories;
using AuthBackend.Services;

namespace AuthBackend.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IRefreshTokenRepository> _refreshRepoMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService(
                _userRepoMock.Object,
                _refreshRepoMock.Object,
                _tokenServiceMock.Object,
                _userRepoMock.Object
            );
        }

        [Fact]
        public async Task Register_ShouldReturnTokens_WhenUserIsNew()
        {
            // Arrange
            var request = new RegisterRequest { Username = "newuser", Password = "123456" };

            _userRepoMock.Setup(r => r.GetByUsernameAsync("newuser")).ReturnsAsync((User?)null);
            _tokenServiceMock.Setup(t => t.GenerateAccessToken(It.IsAny<User>())).Returns("fake_access");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("fake_refresh");
            _tokenServiceMock.Setup(t => t.HashToken("fake_refresh")).Returns("hashed_refresh");

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.Equal("fake_access", result.accessToken);
            Assert.Equal("fake_refresh", result.refreshToken);
        }

        [Fact]
        public async Task Login_ShouldReturnTokens_WhenUserIsValid()
        {
            // Arrange
            var request = new LoginRequest { Username = "validuser", Password = "123456" };
            var fakeUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "validuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            };

            _userRepoMock.Setup(r => r.GetByUsernameAsync("validuser")).ReturnsAsync(fakeUser);
            _tokenServiceMock.Setup(t => t.GenerateAccessToken(fakeUser)).Returns("login_access");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("login_refresh");
            _tokenServiceMock.Setup(t => t.HashToken("login_refresh")).Returns("hashed_login_refresh");

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.Equal("login_access", result.accessToken);
            Assert.Equal("login_refresh", result.refreshToken);
        }
    }
}
