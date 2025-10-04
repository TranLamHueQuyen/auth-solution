using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using AuthBackend.Controllers;
using AuthBackend.Models;
using AuthBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUsernameExists()
    {
        // Arrange
        var request = new RegisterRequest { Username = "duplicateUser", Password = "123456" };
        _authServiceMock.Setup(s => s.RegisterAsync(request))
            .ThrowsAsync(new InvalidOperationException("Username already exists."));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var value = badRequest.Value?.ToString();
        Assert.Contains("Username already exists", value);
    }

    [Fact]
public async Task Register_ReturnsOk_WhenSuccessful()
{
    // Arrange
    var request = new RegisterRequest { Username = "okuser", Password = "123456" };
    _authServiceMock.Setup(s => s.RegisterAsync(request))
        .ReturnsAsync(("ok_access", "ok_refresh"));

    // 👇 Thêm đoạn này để có HttpContext giả
    _controller.ControllerContext = new ControllerContext()
    {
        HttpContext = new DefaultHttpContext()
    };

    // Act
    var result = await _controller.Register(request);

    // Assert
    var ok = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(ok.Value);
    Assert.Contains("ok_access", ok.Value.ToString()!);
}

}
