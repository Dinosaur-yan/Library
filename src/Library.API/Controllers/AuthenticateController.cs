using Library.API.Entities;
using Library.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    /// <summary>
    /// 认证
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        public AuthenticateController(IConfiguration configuration, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            Configuration = configuration;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public IConfiguration Configuration { get; }
        public UserManager<User> UserManager { get; }
        public RoleManager<Role> RoleManager { get; }

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

        [AllowAnonymous]
        [HttpPost("register", Name = nameof(AddUserAsync))]
        public async Task<ActionResult> AddUserAsync([FromBody] RegisterUser registerUser)
        {
            var user = new User
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email,
                BirthDate = registerUser.BirthDate
            };

            IdentityResult result = await UserManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                ModelState.AddModelError("Error", result.Errors.FirstOrDefault()?.Description);
                return BadRequest(ModelState);
            }
        }

        [AllowAnonymous]
        [HttpPost("token2", Name = nameof(GenerateTokenAsync))]
        public async Task<ActionResult> GenerateTokenAsync([FromBody] LoginUser loginUser)
        {
            var user = await UserManager.FindByEmailAsync(loginUser.Email);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = UserManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUser.Password);
            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized();
            }

            var userClaims = await UserManager.GetClaimsAsync(user);
            var userRoles = await UserManager.GetRolesAsync(user);

            foreach (var roleItem in userRoles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, roleItem));
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email)
            };

            claims.AddRange(userClaims);

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
