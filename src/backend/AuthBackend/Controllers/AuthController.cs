using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthBackend.Models;
using AuthBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AuthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

         [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var (access, refresh) = await _authService.RegisterAsync(request);
                SetRefreshTokenCookie(refresh);
                return Ok(new { access });
            }
            catch (InvalidOperationException ex)
            {
                // Trả 400 Bad Request với thông báo chi tiết
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var (access, refresh) = await _authService.LoginAsync(request);
            SetRefreshTokenCookie(refresh);
            return Ok(new { access });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refresh = Request.Cookies["refreshToken"];
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var (access, newRefresh) = await _authService.RefreshAsync(refresh, userId);
            SetRefreshTokenCookie(newRefresh);
            return Ok(new { access });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _authService.LogoutAsync(userId);
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Username = User.Identity?.Name,
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        private void SetRefreshTokenCookie(string token)
        {
            Response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
    }
}
