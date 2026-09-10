using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Common;
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

        public async Task<UserDTO> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken)
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
            user = new User();

            user.UserName = createUserDto.Username;
            user.Email = createUserDto.Email;
            user.RoleId = createUserDto.RoleId;
            user.Name = createUserDto.Name;
            user.CreatedAt = DateTime.Now;
            user.IsDeleted = 0;
            user.LastModifiedDate = DateTime.Now;
            user.PasswordHash = this._passwordHasherService.HashPassword(createUserDto.Password);


            await this._userRepository.AddAsync(user, cancellationToken);

            return new UserDTO
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Role = user.RoleId.ToString()
            };
        }

        public async Task UpdateUserAsync(int userId, UpdateUserDto updateUserDto, CancellationToken cancellationToken)
        {
            // validate the user exists
            var user = await _userRepository.ByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            // validate the username is not used by another user
            user = await _userRepository.GetByUserNameAsync(updateUserDto.Username, cancellationToken);
            if (user != null && user.Id != userId)
            {
                throw new Exception("Username already Exists");
            }

            // validate the email is not used by another user
            user = await _userRepository.GetByEmailAsync(updateUserDto.Email, cancellationToken);
            if (user != null && user.Id != userId)
            {
                throw new Exception("Email already Exists");
            }

            //validate if password & password confirmed is matched if they exists
            if (!String.IsNullOrEmpty(updateUserDto.Password))
            {
                if(updateUserDto.Password != updateUserDto.PasswordConfirmed || String.IsNullOrEmpty(updateUserDto.PasswordConfirmed))
                {
                    throw new InvalidDataException("Password and password confirmeed is not martched");
                }

                user.PasswordHash = _passwordHasherService.HashPassword(updateUserDto.Password);
            }

            user.Name = updateUserDto.Name;
            user.UserName = updateUserDto.Username;
            user.Email = updateUserDto.Email;
            user.RoleId = updateUserDto.RoleId;
            user.LastModifiedDate = DateTime.Now;

            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task DeleteUserAsync(int userId, CancellationToken cancellationToken)
        {
            // validate the user exists
            var user = await _userRepository.ByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            await _userRepository.DeleteAsync(userId, cancellationToken);
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
