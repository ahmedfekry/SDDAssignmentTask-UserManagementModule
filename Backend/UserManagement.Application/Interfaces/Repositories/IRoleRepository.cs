using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(int id,CancellationToken cancellationToken);
        public Task AddAsync(Role user,CancellationToken cancellationToken);
        public Task UpdateAsync(Role user, CancellationToken cancellationToken);
        public Task DeleteAsync(int id, CancellationToken cancellationToken);
        public Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
    }
}
