using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Common;
using UserManagement.Application.Common.Models;
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
            return await this._applicationDbContext.Users.Include(u => u.Role).Where(u => u.Id == id).FirstOrDefaultAsync();
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

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(UserQueryOptions options, CancellationToken cancellationToken)
        {
            IQueryable<User> query = this._applicationDbContext.Users.Include(usr => usr.Role);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                var pattern = $"%{options.Search.Trim()}%";
                query = query.Where(u =>
                    EF.Functions.Like(u.Name, pattern) ||
                    EF.Functions.Like(u.UserName, pattern) ||
                    EF.Functions.Like(u.Email, pattern));
            }

            if (!string.IsNullOrWhiteSpace(options.Role))
            {
                query = query.Where(u => u.Role.Name == options.Role);
            }

            var descending = string.Equals(options.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            IOrderedQueryable<User> ordered = options.SortBy?.ToLowerInvariant() switch
            {
                "username" => descending ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
                "email" => descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "role" => descending ? query.OrderByDescending(u => u.Role.Name) : query.OrderBy(u => u.Role.Name),
                _ => descending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name)
            };

            // Tie-breaker so pagination stays stable/deterministic even when the sort
            // column has duplicate values across rows.
            query = ordered.ThenBy(u => u.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .Skip((options.Page - 1) * options.PageSize)
                .Take(options.PageSize)
                .ToListAsync(cancellationToken);

            return (users, totalCount);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await this._applicationDbContext.Users.Where(usr => usr.Email == email).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            var user = await this._applicationDbContext.Users.Include(usr => usr.Role).Where(usr => usr.UserName ==  userName).FirstOrDefaultAsync();
            return user;
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var existingUser = await this._applicationDbContext.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new Exception("Not Found");
            }

            this._applicationDbContext.Users.Update(user);
            await this._applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
