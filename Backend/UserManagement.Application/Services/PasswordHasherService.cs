using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.Application.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private const int saltSize = 16;
        private const int hashSize = 32;
        private const int Iterations = 350000;
        private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;
        private const char SegmentDelimiter = ':';
        public bool CheckPassword(string password, string hashedPassword)
        {
            var segments = hashedPassword.Split(SegmentDelimiter);
            if (segments.Length != 4)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(segments[0]);
            byte[] hash = Convert.FromBase64String(segments[1]);

            if (!int.TryParse(segments[2], out int iterations))
            {
                return false;
            }

            var algorithm = new HashAlgorithmName(segments[3]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                algorithm,
                hash.Length);

            // FixedTimeEquals prevents timing attacks during string comparison
            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }

        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                _hashAlgorithm,
                hashSize);

            // Format: {salt}:{hash}:{iterations}:{algorithm}
            return string.Join(
                SegmentDelimiter,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash),
                Iterations,
                _hashAlgorithm.Name);
        }
    }
}
