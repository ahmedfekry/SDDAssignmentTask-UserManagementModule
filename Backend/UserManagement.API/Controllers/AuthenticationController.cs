using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using UserManagement.API.Middleware;
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

                SetAuthCookies(result.JwtToken.Token, result.JwtToken.ExpiresAt);

                return Success(new
                {
                    userId = result.UserId,
                    username = result.Username,
                    roleName = result.RoleName,
                    expiresAt = result.JwtToken.ExpiresAt
                }, "Login Successful");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message,StatusCodes.Status404NotFound, []);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(AuthCookieNames.AccessToken, CookiePath());
            Response.Cookies.Delete(XsrfValidationMiddleware.CookieName, CookiePath());

            return Success(new { }, "Logged out");
        }

        private void SetAuthCookies(string token, DateTime expiresAt)
        {
            Response.Cookies.Append(AuthCookieNames.AccessToken, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiresAt,
                Path = "/"
            });

            var csrfToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            Response.Cookies.Append(XsrfValidationMiddleware.CookieName, csrfToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiresAt,
                Path = "/"
            });
        }

        private static CookieOptions CookiePath() => new()
        {
            Path = "/",
            Secure = true,
            SameSite = SameSiteMode.None
        };
    }
}
