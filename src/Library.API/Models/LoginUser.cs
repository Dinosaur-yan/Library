namespace Library.API.Models
{
    public class LoginUser
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public const string demoName = "demouser";

        public const string demoPwd = "demopwd";
    }
}
