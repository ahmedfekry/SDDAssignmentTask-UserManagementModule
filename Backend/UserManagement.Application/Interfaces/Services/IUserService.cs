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
        Task CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken);
    }
}
