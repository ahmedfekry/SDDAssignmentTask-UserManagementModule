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
        Task<Role> GetByIdAsync(int id);
        public Task AddAsync(User user);
        public Task UpdateAsync(User user);
        public Task DeleteAsync(int id);
        public Task<IEnumerable<User>> GetAllAsync();
    }
}
