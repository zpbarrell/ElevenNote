using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElevenNote.Models.User
{
    public class UserRegister
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(4)]
        public string Username { get; set; }
        [Required]
        [MinLength(4)]
        public string Password { get; set; }
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}