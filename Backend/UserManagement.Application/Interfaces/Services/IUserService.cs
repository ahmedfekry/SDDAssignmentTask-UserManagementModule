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
        Task CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken);
        Task DeleteUserAsync(string userId,CancellationToken cancellationToken);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync(CancellationToken cancellationToken);
    }
}
