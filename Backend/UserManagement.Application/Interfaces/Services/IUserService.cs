using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.DTOs.User;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDTO> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken);
        Task UpdateUserAsync(int userId, UpdateUserDto updateUserDto, CancellationToken cancellationToken);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task DeleteUserAsync(int userId,CancellationToken cancellationToken);
    }
}
