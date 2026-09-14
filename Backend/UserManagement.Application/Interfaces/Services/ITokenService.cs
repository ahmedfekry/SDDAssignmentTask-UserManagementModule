using System;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Application.Common.Models;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces.Services
{
    public interface ITokenService
    {
        JWTToken GenerateAccessToken(User user);
        Task<(string PlainToken, DateTime ExpiresAt)> IssueRefreshTokenAsync(int userId, Guid familyId, CancellationToken cancellationToken);
        Task<RefreshResult> RotateAsync(string presentedPlainToken, CancellationToken cancellationToken);
        Task RevokeAsync(string presentedPlainToken, CancellationToken cancellationToken);
    }
}
