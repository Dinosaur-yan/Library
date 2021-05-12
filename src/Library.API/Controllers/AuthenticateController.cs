using Library.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Library.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        public AuthenticateController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [AllowAnonymous]
        [HttpPost("token", Name = nameof(GenerateToken))]
        public ActionResult GenerateToken([FromBody] LoginUser loginUser)
        {
            if (loginUser == null || loginUser.UserName.ToLower() != LoginUser.demoName || loginUser.Password.ToLower() != LoginUser.demoPwd)
            {
                return Unauthorized();
            }

            return Ok();
        }
    }
}
