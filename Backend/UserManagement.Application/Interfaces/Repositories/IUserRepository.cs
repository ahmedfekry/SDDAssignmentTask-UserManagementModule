using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        public Task<User> ByIdAsync(int id, CancellationToken cancellationToken);
        public Task AddAsync(User user, CancellationToken cancellationToken);
        public Task UpdateAsync(User user, CancellationToken cancellationToken);
        public Task DeleteAsync(int id, CancellationToken cancellationToken);
        public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
        public Task<User> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
    }
}
