using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Common;
using UserManagement.Domain.DTOs.User;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDTO> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken);
        Task UpdateUserAsync(int userId, UpdateUserDto updateUserDto, CancellationToken cancellationToken);
        Task<PagedResult<UserDTO>> GetUsersPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<UserDTO> GetUserByIdAsync(int userId, CancellationToken cancellationToken);
        Task DeleteUserAsync(int userId,CancellationToken cancellationToken);
    }
}
