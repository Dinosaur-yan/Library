using System;
using System.ComponentModel.DataAnnotations;

namespace Library.API.Models
{
    public class RegisterUser
    {
        [Required, MinLength(4)]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string RoleName { get; set; }

        public DateTimeOffset BirthDate { get; set; }
    }
}
