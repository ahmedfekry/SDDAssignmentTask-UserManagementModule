using System.Linq;

namespace UserManagement.API.Middleware
{
    /// <summary>
    /// Double-submit-cookie CSRF check. The access token lives in an HttpOnly cookie the
    /// browser attaches automatically, so any mutating request needs a second proof that it
    /// was issued by our own frontend JS (which can read the non-HttpOnly XSRF-TOKEN cookie)
    /// rather than by a third-party site riding the browser's cookie jar.
    /// </summary>
    public class XsrfValidationMiddleware
    {
        public const string CookieName = "XSRF-TOKEN";
        public const string HeaderName = "X-XSRF-TOKEN";

        private static readonly HashSet<string> SafeMethods = new(StringComparer.OrdinalIgnoreCase)
        {
            "GET", "HEAD", "OPTIONS"
        };

        private readonly RequestDelegate _next;

        public XsrfValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var isMutatingRequest = !SafeMethods.Contains(context.Request.Method);
            var hasSession = context.Request.Cookies.ContainsKey(AuthCookieNames.AccessToken);

            if (isMutatingRequest && hasSession)
            {
                var cookieToken = context.Request.Cookies[CookieName];
                var headerToken = context.Request.Headers[HeaderName].FirstOrDefault();

                if (string.IsNullOrEmpty(cookieToken) || string.IsNullOrEmpty(headerToken) || cookieToken != headerToken)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Invalid or missing CSRF token",
                        result = (object?)null,
                        errors = Array.Empty<string>()
                    });
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class AuthCookieNames
    {
        public const string AccessToken = "access_token";
    }
}
