using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Common;
using UserManagement.Application.Common.Models;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services
{
    public class JWTGeneratorService : IJWTGeneratorService
    {
        // A distinct audience from the access token's - this is what stops a refresh
        // token from being usable as a Bearer token against protected endpoints, since
        // the standard JwtBearer scheme validates against jwtSettings.Audience only.
        private const string RefreshAudience = "UserManagement.Client.Refresh";

        private readonly JwtSettings _settings;

        public JWTGeneratorService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }
        public JWTToken GenerateJWTTekenAsync(User user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name), // required for [Authorize(Roles = "Admin")] to work
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JWTToken { Token = new JwtSecurityTokenHandler().WriteToken(token).ToString(),  ExpiresAt = expiresAt};
        }

        public JWTToken GenerateRefreshToken(User user)
        {
            var expiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: RefreshAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JWTToken { Token = new JwtSecurityTokenHandler().WriteToken(token), ExpiresAt = expiresAt };
        }

        public int? ValidateRefreshTokenAndGetUserId(string refreshToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

            try
            {
                var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = RefreshAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                return int.TryParse(sub, out var userId) ? userId : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
