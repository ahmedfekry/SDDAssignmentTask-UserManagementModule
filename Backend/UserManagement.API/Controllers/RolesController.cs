using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : BaseApiController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _roleService.GetRolesAsync(cancellationToken);

                return Success(roles);
            }
            catch (Exception ex)
            {
                return Failed(ex.Message);
            }
        }

    }
}
