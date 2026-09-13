using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
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
        private const string AuditTableName = "Users";

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ICurrentUserService _currentUserService;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasherService,
            IAuditLogRepository auditLogRepository,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _auditLogRepository = auditLogRepository;
            _currentUserService = currentUserService;
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            // validate existing Username
            var user = await _userRepository.GetByUserNameAsync(createUserDto.Username, cancellationToken);
            if (user != null)
            {
                throw new Exception("Username already Exists");
            }

            //validate the email exists
            user = await _userRepository.GetByEmailAsync(createUserDto.Email, cancellationToken);
            if (user != null)
            {
                throw new Exception("Username already Exists");

            }

            //validate the password and passwordConfirmed is matched
            if(createUserDto.Password != createUserDto.PasswordConfirmed)
            {
                throw new InvalidDataException("Password and password confirmed is not matched");
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

            await WriteAuditAsync(ActionType.Add, user.Id, oldValues: null, newValues: GetJsonValueOfObject(user), cancellationToken);

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
            var userWithSameUsername = await _userRepository.GetByUserNameAsync(updateUserDto.Username, cancellationToken);
            if (userWithSameUsername != null && userWithSameUsername.Id != userId)
            {
                throw new Exception("Username already Exists");
            }

            // validate the email is not used by another user
            var userWithSameEmail = await _userRepository.GetByEmailAsync(updateUserDto.Email, cancellationToken);
            if (userWithSameEmail != null && userWithSameEmail.Id != userId)
            {
                throw new Exception("Email already Exists");
            }

            // validate the user is admin if he is updating another user
            if(_currentUserService.UserId != user.Id)
            {
                var currentUser = await _userRepository.ByIdAsync(_currentUserService.UserId.Value, cancellationToken);
                if(currentUser.Role.Name != "Admin")
                {
                    throw new UnauthorizedAccessException("You are not allowed to update this user");
                }
            }

            var oldValues = GetJsonValueOfObject(user);

            //validate if password & password confirmed is matched if they exists
            if (!String.IsNullOrEmpty(updateUserDto.Password))
            {
                if (updateUserDto.Password != updateUserDto.PasswordConfirmed || String.IsNullOrEmpty(updateUserDto.PasswordConfirmed))
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

            await WriteAuditAsync(ActionType.Update, user.Id, oldValues, GetJsonValueOfObject(user), cancellationToken);
        }

        public async Task DeleteUserAsync(int userId, CancellationToken cancellationToken)
        {
            // validate the user exists
            var user = await _userRepository.ByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var oldValues = GetJsonValueOfObject(user);

            await _userRepository.DeleteAsync(userId, cancellationToken);

            await WriteAuditAsync(ActionType.Delete, userId, oldValues, newValues: null, cancellationToken);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return users.Select(user => new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.UserName,
                Role = user.Role.Name,
                RoleId = user.RoleId
            });
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.ByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.UserName,
                Role = user.Role?.Name,
                RoleId = user.RoleId
            };
        }

        public string GetJsonValueOfObject(User user)
        {
            return JsonSerializer.Serialize(new
            {
                user.Id,
                user.Name,
                user.UserName,
                user.Email,
                user.RoleId,
                user.IsDeleted
            });
        }

        private async Task WriteAuditAsync(
            ActionType actionType,
            int entityId,
            string? oldValues,
            string? newValues,
            CancellationToken cancellationToken)
        {
            var auditLog = new AuditLog
            {
                TableName = AuditTableName,
                actionType = actionType,
                EntityId = entityId,
                CreatedAt = DateTime.Now,
                CreatedBy = _currentUserService.UserId ?? 0,
                IpAddress = _currentUserService.IpAddress ?? string.Empty,
                OldEntityValues = oldValues,
                NewEntityValues = newValues,
                ChangeDetails = $"{actionType} on {AuditTableName} (Id: {entityId})"
            };

            await _auditLogRepository.CreateAsync(auditLog, cancellationToken);
        }
    }
}
