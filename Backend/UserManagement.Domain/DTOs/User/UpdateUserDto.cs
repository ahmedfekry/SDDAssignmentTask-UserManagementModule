using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Domain.DTOs.User
{
    public class UpdateUserDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [AllowedValues([1, 2, 3], ErrorMessage = "Invalid Role Id")]
        public int RoleId { get; set; }

        [StringLength(8,ErrorMessage = "Password must be greater than 8 char")]
        public string? Password {get; set;}
        [StringLength(8,ErrorMessage = "Password must be greater than 8 char")]
        public string? PasswordConfirmed { get; set; }
        public int ModifiedBy { get; set; }
    }
}
