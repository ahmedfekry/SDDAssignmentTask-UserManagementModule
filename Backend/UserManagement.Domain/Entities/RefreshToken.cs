using System;

namespace UserManagement.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        // Never store the raw token - only its hash, same principle as a password.
        public string TokenHash { get; set; }

        // Every token issued from the same login (through rotation) shares a FamilyId.
        // If a token is presented after it's already been rotated away, that's a signal
        // of theft/replay, and the whole family gets revoked.
        public Guid FamilyId { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;
    }
}
