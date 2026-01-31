using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        [Range(18, 120, ErrorMessage = "You must be at least 18 years old.")]
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
