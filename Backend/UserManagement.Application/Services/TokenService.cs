using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Application.Common;
using UserManagement.Application.Common.Models;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJWTGeneratorService _jwtGeneratorService;
        private readonly JwtSettings _settings;

        public TokenService(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IJWTGeneratorService jwtGeneratorService,
            IOptions<JwtSettings> settings)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _jwtGeneratorService = jwtGeneratorService;
            _settings = settings.Value;
        }

        public JWTToken GenerateAccessToken(User user) => _jwtGeneratorService.GenerateJWTTekenAsync(user);

        public async Task<(string PlainToken, DateTime ExpiresAt)> IssueRefreshTokenAsync(int userId, Guid familyId, CancellationToken cancellationToken)
        {
            var plainToken = GenerateOpaqueToken();
            var expiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays);

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = userId,
                TokenHash = Hash(plainToken),
                FamilyId = familyId,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            return (plainToken, expiresAt);
        }

        public async Task<RefreshResult> RotateAsync(string presentedPlainToken, CancellationToken cancellationToken)
        {
            var hash = Hash(presentedPlainToken);
            var existing = await _refreshTokenRepository.GetByHashAsync(hash, cancellationToken);

            if (existing == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (existing.RevokedAt != null)
            {
                // This token was already rotated away once before - somebody is replaying an
                // old token (stolen cookie, race between tabs, etc). Kill the whole chain.
                await _refreshTokenRepository.RevokeFamilyAsync(existing.FamilyId, cancellationToken);
                throw new UnauthorizedAccessException("Refresh token reuse detected");
            }

            if (existing.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token expired");
            }

            var user = await _userRepository.ByIdAsync(existing.UserId, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var (newPlainToken, newExpiresAt) = await IssueRefreshTokenAsync(existing.UserId, existing.FamilyId, cancellationToken);

            existing.RevokedAt = DateTime.UtcNow;
            existing.ReplacedByTokenHash = Hash(newPlainToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return new RefreshResult
            {
                User = user,
                AccessToken = GenerateAccessToken(user),
                RefreshToken = newPlainToken,
                RefreshTokenExpiresAt = newExpiresAt
            };
        }

        public async Task RevokeAsync(string presentedPlainToken, CancellationToken cancellationToken)
        {
            var hash = Hash(presentedPlainToken);
            var existing = await _refreshTokenRepository.GetByHashAsync(hash, cancellationToken);
            if (existing != null && existing.RevokedAt == null)
            {
                existing.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
            }
        }

        private static string GenerateOpaqueToken() => ToUrlSafeBase64(RandomNumberGenerator.GetBytes(32));

        private static string Hash(string token) => ToUrlSafeBase64(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        private static string ToUrlSafeBase64(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
