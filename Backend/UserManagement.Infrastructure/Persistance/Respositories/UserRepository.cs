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
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await this._applicationDbContext.Users.AddAsync(user);
            this._applicationDbContext.SaveChanges();
        }

        public async Task<User?> ByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await this._applicationDbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var user = await this._applicationDbContext.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("Not Found");
            }

            this._applicationDbContext.Users.Remove(user);
            await this._applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await this._applicationDbContext.Users.ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var existingUser = await this._applicationDbContext.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new Exception("Not Found");
            }

            this._applicationDbContext.Update(user);
            await this._applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
