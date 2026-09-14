using System;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Common.Models
{
    public class RefreshResult
    {
        public User User { get; set; } = null!;
        public JWTToken AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
