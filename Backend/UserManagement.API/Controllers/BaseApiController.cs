using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common;

namespace UserManagement.API.Controllers
{
    public class BaseApiController : ControllerBase
    {
        protected IActionResult Success<T>(T? data, string message = "Success")
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        protected IActionResult Failed(string message = "Failed",int statusCode = StatusCodes.Status400BadRequest, IEnumerable<string> errors = null)
        {
            return StatusCode(statusCode, ApiResponse<object?>.FailureResponse(message, errors));
        }

    }
}
