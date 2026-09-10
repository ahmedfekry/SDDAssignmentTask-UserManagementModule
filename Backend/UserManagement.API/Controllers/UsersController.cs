using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.DTOs.User;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.CreateUser(createUserDto,cancellationToken);
                return Success(new { }, "Success");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }

        }
    }
}
