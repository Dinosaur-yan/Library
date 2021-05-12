using Library.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,loginUser.UserName)
            };

            var tokenSection = Configuration.GetSection("Security:Token");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSection["Key"]));
            var signCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: tokenSection["Issuer"],
                audience: tokenSection["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),   //jwt不支持销毁及撤回功能，因此设置有效时间时，应设置一个较短的时间
                signingCredentials: signCredential);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = TimeZoneInfo.ConvertTimeFromUtc(jwtToken.ValidTo, TimeZoneInfo.Local)
            });
        }
    }
}
