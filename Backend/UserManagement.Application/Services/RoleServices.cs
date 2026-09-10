using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services
{
    public class RoleServices : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleServices(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<IEnumerable<Role>> GetRolesAsync(CancellationToken cancellationToken)
        {
            return await this._roleRepository.GetAllAsync(cancellationToken);
        }
    }
}
