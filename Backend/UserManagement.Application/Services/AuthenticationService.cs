using System;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Application.Common;
using UserManagement.Application.Common.Models;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IUserRepository userRepository,
                                     IPasswordHasherService passwordHasherService,
                                     ITokenService tokenService
                                     )
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var user = await this._userRepository.GetByUserNameAsync(loginRequest.Username,cancellationToken);
            if (user == null)
                throw new NotFoundException("Invalid Login Credentials");

            var passwordValid = _passwordHasherService.CheckPassword(loginRequest.Password, user.PasswordHash);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException();
            }

            var (refreshToken, refreshExpiresAt) = await _tokenService.IssueRefreshTokenAsync(user.Id, Guid.NewGuid(), cancellationToken);

            return new LoginResponse
            {
                Username = user.UserName,
                UserId = user.Id,
                RoleName = user.Role.Name,
                JwtToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshExpiresAt
            };
        }

        public async Task<LoginResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var result = await _tokenService.RotateAsync(refreshToken, cancellationToken);

            return new LoginResponse
            {
                Username = result.User.UserName,
                UserId = result.User.Id,
                RoleName = result.User.Role.Name,
                JwtToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                RefreshTokenExpiresAt = result.RefreshTokenExpiresAt
            };
        }

        public Task LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        {
            return _tokenService.RevokeAsync(refreshToken, cancellationToken);
        }
    }
}
