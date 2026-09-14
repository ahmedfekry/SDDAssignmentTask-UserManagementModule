using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Common.Models;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IJWTGeneratorService
    {
        JWTToken GenerateJWTTekenAsync(User user);
        JWTToken GenerateRefreshToken(User user);
        int? ValidateRefreshTokenAndGetUserId(string refreshToken);
    }
}
