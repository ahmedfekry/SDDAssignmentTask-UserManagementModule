using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Common;
using UserManagement.Application.Common.Models;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IJWTGeneratorService _jWTGeneratorService;

        public AuthenticationService(IUserRepository userRepository,
                                     IPasswordHasherService passwordHasherService,
                                     IJWTGeneratorService jWTGeneratorService
                                     ) 
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _jWTGeneratorService = jWTGeneratorService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var user = new User();

            user = await this._userRepository.GetByUserNameAsync(loginRequest.Username,cancellationToken);
            if (user == null)
                throw new NotFoundException("Invalid Login Credentials");

            var passwordValid = _passwordHasherService.CheckPassword(loginRequest.Password, user.PasswordHash);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException();
            }

            return BuildLoginResponse(user, includeRefreshToken: true);
        }

        public async Task<LoginResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var userId = _jWTGeneratorService.ValidateRefreshTokenAndGetUserId(refreshToken);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var user = await _userRepository.ByIdAsync(userId.Value, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            // Not rotated (stateless refresh tokens) - the same refresh token keeps working
            // until it naturally expires, so we only mint a fresh access token here.
            return BuildLoginResponse(user, includeRefreshToken: false);
        }

        private LoginResponse BuildLoginResponse(User user, bool includeRefreshToken)
        {
            return new LoginResponse
            {
                Username = user.UserName,
                UserId = user.Id,
                RoleName = user.Role.Name,
                JwtToken = _jWTGeneratorService.GenerateJWTTekenAsync(user),
                RefreshToken = includeRefreshToken ? _jWTGeneratorService.GenerateRefreshToken(user) : null
            };
        }
    }
}
