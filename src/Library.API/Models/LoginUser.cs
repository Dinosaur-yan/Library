﻿using System.ComponentModel.DataAnnotations;

namespace Library.API.Models
{
    public class LoginUser
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public const string demoName = "demouser";

        public const string demoPwd = "demopwd";
    }
}
