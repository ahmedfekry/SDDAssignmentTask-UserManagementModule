using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody]CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.CreateUserAsync(createUserDto,cancellationToken);
                return Success(result, "Success");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto updateUserDto, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.UpdateUserAsync(id, updateUserDto, cancellationToken);
                return Success(new { }, "Success");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeleteUserAsync(id, cancellationToken);
                return Success(new { }, "Success");
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,User,ReadOnlyUser")]
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
