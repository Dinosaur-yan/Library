using System;

namespace Library.API.Models
{
    public class TokenResult
    {
        public DateTimeOffset Expiration { get; set; }

        public string Token { get; set; }
    }
}
