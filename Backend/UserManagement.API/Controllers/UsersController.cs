using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
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
                await _userService.CreateUserAsync(createUserDto,cancellationToken);
                return Success(new { }, "Success");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(cancellationToken);
                return Success(new { users });
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }
        }
    }
}
