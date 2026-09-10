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

                return Success(result, "Login Successful");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message,StatusCodes.Status404NotFound, []);
            }
        }

       
    }
}
