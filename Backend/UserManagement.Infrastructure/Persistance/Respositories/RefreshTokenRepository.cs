using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistance.Respositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public RefreshTokenRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
        {
            await _applicationDbContext.RefreshTokens.AddAsync(token, cancellationToken);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
        }

        public async Task RevokeFamilyAsync(Guid familyId, CancellationToken cancellationToken)
        {
            var tokens = await _applicationDbContext.RefreshTokens
                .Where(t => t.FamilyId == familyId && t.RevokedAt == null)
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            foreach (var token in tokens)
            {
                token.RevokedAt = now;
            }

            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
