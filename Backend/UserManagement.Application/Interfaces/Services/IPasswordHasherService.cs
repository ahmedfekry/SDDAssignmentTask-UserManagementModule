using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool CheckPassword(string password, string hashedPassword);
    }
}
