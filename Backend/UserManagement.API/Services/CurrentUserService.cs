using System.Security.Claims;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

                return int.TryParse(value, out var id) ? id : null;
            }
        }

        public string? IpAddress
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext is null)
                {
                    return null;
                }

                // Prefer the forwarded header when the API sits behind a proxy / load balancer.
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                return httpContext.Connection.RemoteIpAddress?.ToString();
            }
        }
    }
}
