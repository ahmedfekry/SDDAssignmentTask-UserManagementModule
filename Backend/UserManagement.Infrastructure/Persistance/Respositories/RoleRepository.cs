using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistance.Respositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public RoleRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(Role role, CancellationToken cancellationToken)
        {
            await this._applicationDbContext.Roles.AddAsync(role);
            this._applicationDbContext.SaveChanges();
        }

        public async Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await this._applicationDbContext.Roles.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var role = await this._applicationDbContext.Roles.FindAsync(id);
            if (role == null)
            {
                throw new Exception("Not Found");
            }

            this._applicationDbContext.Roles.Remove(role);
            await this._applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await this._applicationDbContext.Roles.ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            var existingRole = await this._applicationDbContext.Roles.FindAsync(role.Id);
            if (existingRole == null)
            {
                throw new Exception("Not Found");
            }

            this._applicationDbContext.Roles.Update(role);
            await this._applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
