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

        public AuthenticationService(IUserRepository userRepository,IPasswordHasherService passwordHasherService) 
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var user = new User();
            try
            {
                user = await this._userRepository.GetByUserNameAsync(loginRequest.Username,cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }

            var passwordValid = _passwordHasherService.CheckPassword(loginRequest.Password, user.PasswordHash);
            if (passwordValid)
            {
                throw new UnauthorizedAccessException();
            }

            return new LoginResponse
            {
                Token = "Test Token",
                ExpiresAt = DateTime.UtcNow
            };
        }
    }
}
