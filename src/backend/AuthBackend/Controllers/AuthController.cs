using System;
using System.Threading.Tasks;
using AuthBackend.Models;
using AuthBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using AuthBackend.Models;

namespace AuthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHostEnvironment _env;

        public AuthController(IAuthService authService, IHostEnvironment env)
        {
            _authService = authService;
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var tokens = await _authService.RegisterAsync(request);
            SetRefreshTokenCookie(tokens.refreshToken);
            return Ok(new { tokens.accessToken });
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
            if (!Request.Cookies.TryGetValue("refreshToken", out var refresh) || string.IsNullOrEmpty(refresh))
                return Unauthorized();

            var tokens = await _authService.RefreshAsync(refresh);
            SetRefreshTokenCookie(tokens.refreshToken);
            return Ok(new { tokens.accessToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Guid userId)
        {
            await _authService.LogoutAsync(userId);
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }

        private void SetRefreshTokenCookie(string token)
        {
            var isProd = _env.IsProduction();
            Response.Cookies.Append("refreshToken", token, new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true,
                Secure = isProd,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
    }
}
