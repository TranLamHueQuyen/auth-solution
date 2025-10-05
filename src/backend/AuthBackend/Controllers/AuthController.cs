// Controllers/AuthController.cs
using AuthBackend.Models;
using AuthBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
                var tokens = await _authService.RegisterAsync(request);
                SetRefreshTokenCookie(tokens.refreshToken);
                return Ok(new { accessToken = tokens.accessToken });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var (access, refresh) = await _authService.LoginAsync(request);

                // Lưu refresh token vào cookie HttpOnly (7 ngày)
                Response.Cookies.Append("refreshToken", refresh, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new { accessToken = access });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refresh))
                return Unauthorized();

            var tokens = await _authService.RefreshAsync(refresh);
            SetRefreshTokenCookie(tokens.refreshToken);
            return Ok(new { accessToken = tokens.accessToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Guid? userId)
        {
            if (userId.HasValue)
            {
                await _authService.LogoutAsync(userId.Value);
            }
            else
            {
                // nếu không có userId, thử lấy từ token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var id))
                    await _authService.LogoutAsync(id);
            }

            Response.Cookies.Delete("refreshToken");
            return Ok();
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var user = await _authService.GetUserByIdAsync(Guid.Parse(userIdClaim));
            if (user == null)
                return NotFound();

            return Ok(new { user.Id, user.Username, user.Role });
        }

        private void SetRefreshTokenCookie(string token)
        {
            Response.Cookies.Append("refreshToken", token, new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
    }
}