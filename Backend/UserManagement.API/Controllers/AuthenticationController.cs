using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Services;
using UserManagement.Application.Common.Models;
using UserManagement.Application.Common;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseApiController
    {
        private const string RefreshCookieName = "refresh_token";
        private const string RefreshCookiePath = "/api/Authentication/refresh";

        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authenticationService.LoginAsync(request, cancellationToken);

                SetRefreshTokenCookie(result.RefreshToken!);

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
                var result = await _authenticationService.RefreshAsync(refreshToken, cancellationToken);

                return Success(BuildAuthPayload(result), "Success");
            }
            catch (Exception)
            {
                // The refresh token is gone/invalid either way, so drop it rather than
                // leaving a dead cookie around for the browser to keep resending.
                Response.Cookies.Delete(RefreshCookieName, CookieOptionsFor(RefreshCookiePath));
                return Failed("Invalid or expired refresh token", StatusCodes.Status401Unauthorized, []);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(RefreshCookieName, CookieOptionsFor(RefreshCookiePath));

            return Success(new { }, "Logged out");
        }

        private static object BuildAuthPayload(LoginResponse result) => new
        {
            accessToken = result.JwtToken.Token,
            expiresAt = result.JwtToken.ExpiresAt,
            userId = result.UserId,
            username = result.Username,
            roleName = result.RoleName
        };

        private void SetRefreshTokenCookie(JWTToken refreshToken)
        {
            Response.Cookies.Append(RefreshCookieName, refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = refreshToken.ExpiresAt,
                // Scoped so the browser only ever sends this cookie to the refresh
                // endpoint itself, not on every request to the API.
                Path = RefreshCookiePath
            });
        }

        private static CookieOptions CookieOptionsFor(string path) => new()
        {
            Path = path,
            Secure = true,
            SameSite = SameSiteMode.None
        };
    }
}
