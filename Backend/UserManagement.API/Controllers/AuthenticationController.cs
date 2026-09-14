using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common.Models;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : BaseApiController
    {
        private const string RefreshCookieName = "refreshToken";
        private const string RefreshCookiePath = "/api/auth";

        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            IUserService userService,
            ICurrentUserService currentUserService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authenticationService.LoginAsync(request, cancellationToken);

                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);

                return Success(BuildAuthPayload(result), "Login Successful");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message, StatusCodes.Status401Unauthorized, []);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Failed("No refresh token", StatusCodes.Status401Unauthorized, []);
            }

            try
            {
                // Rotation: this call also revokes the presented token and issues a new one.
                var result = await _authenticationService.RefreshAsync(refreshToken, cancellationToken);

                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);

                return Success(BuildAuthPayload(result), "Success");
            }
            catch (Exception ex)
            {
                Response.Cookies.Delete(RefreshCookieName, CookieOptionsFor());
                return Failed(ex.Message, StatusCodes.Status401Unauthorized, []);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
            {
                await _authenticationService.LogoutAsync(refreshToken, cancellationToken);
            }

            Response.Cookies.Delete(RefreshCookieName, CookieOptionsFor());

            return Success(new { }, "Logged out");
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(_currentUserService.UserId!.Value, cancellationToken);
                return Success(user, "Success");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message, StatusCodes.Status401Unauthorized, []);
            }
        }

        private static object BuildAuthPayload(LoginResponse result) => new
        {
            accessToken = result.JwtToken.Token,
            expiresIn = (int)Math.Max(0, (result.JwtToken.ExpiresAt - DateTime.UtcNow).TotalSeconds),
            userId = result.UserId,
            username = result.Username,
            roleName = result.RoleName
        };

        private void SetRefreshTokenCookie(string token, DateTime expiresAt)
        {
            Response.Cookies.Append(RefreshCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                // Strict would be dropped entirely here: the frontend runs on http://localhost:4200
                // (no SSL in ng serve) while the API is https://localhost:7254 - that's a scheme
                // mismatch, which Chrome's Schemeful-Same-Site treats as cross-site even though the
                // hostname matches, and Strict/Lax cookies are never sent cross-site. None is the
                // only mode that actually works for a separate-origin API in this setup.
                SameSite = SameSiteMode.None,
                Expires = expiresAt,
                // Scoped so the browser only ever sends this cookie to auth endpoints.
                Path = RefreshCookiePath
            });
        }

        private static CookieOptions CookieOptionsFor() => new()
        {
            Path = RefreshCookiePath,
            Secure = true,
            SameSite = SameSiteMode.None
        };
    }
}
