using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Common.Models
{
    public class LoginResponse
    {
        public JWTToken Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;

    }

    public class JWTToken
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
