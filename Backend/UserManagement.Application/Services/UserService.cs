using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.DTOs.User;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;

        public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasherService)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
        }

        public async Task CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            // validate existing Username
            var user = new User();
            user = await _userRepository.GetByUserNameAsync(createUserDto.Username,cancellationToken);
            if (user != null)
            {
                throw new Exception("Username already Exists");
            }

            //validate the email exists
            user = await _userRepository.GetByEmailAsync(createUserDto.Email,cancellationToken);
            if (user != null)
            {
                throw new Exception("Username already Exists");

            }

            user.UserName = createUserDto.Username;
            user.Email = createUserDto.Email;
            user.RoleId = createUserDto.RoleId;
            user.Name = createUserDto.Name;
            user.CreatedAt = DateTime.Now;
            user.IsDeleted = 0;
            user.LastModifiedDate = DateTime.Now;
            user.PasswordHash = this._passwordHasherService.HashPassword(createUserDto.Password);


            await this._userRepository.AddAsync(user, cancellationToken);

        }

        public Task CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return users.Select(user => new UserDTO
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Username = user.UserName,
                Role = user.Role.Name
            });
        }
    }
}
