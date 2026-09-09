using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public AuthenticationService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public Task<LoginResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            try
            {
                var user = this._userRepository.GetByUserNameAsync(loginRequest.Username,cancellationToken);
            }
            catch (NotFoundException ex)
            {
                throw;
            }

            throw new NotImplementedException();    
        }
    }
}
