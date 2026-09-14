using System;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
        Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);
        Task RevokeFamilyAsync(Guid familyId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
