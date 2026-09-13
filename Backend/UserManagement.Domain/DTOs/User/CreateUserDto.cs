using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.DTOs.User
{
    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(8,ErrorMessage = "Password must be greater than 8 char")]
        public string Password { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "PasswordConfirmed must be greater than 8 char")]
        public string PasswordConfirmed { get; set; }
        [Required]
        [AllowedValues([1,2,3],ErrorMessage = "Invalid Role Id")]
        public int RoleId { get; set; }
        public int CreatedBy { get; set; }
    }
}
