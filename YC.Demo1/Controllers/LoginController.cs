using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YC.Demo1.Configs;
using YC.Demo1.Models;
using YC.Demo1.Models;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace YC.Demo1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<LoginController> _logger;
        private readonly IUserRepository _user;

        public LoginController(
            ILogger<LoginController> logger,
            IConfiguration configuratio,
            IUserRepository user)
        {
            _logger = logger;
            _config = configuratio;
            _user = user;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Login login) 
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress;
            //if (ValidateUser(login))
            (bool IsSuccess, Models.User data) resp = await _user.CheckAccount(login);
            if (resp.IsSuccess == true)
            {
                _logger.LogInformation(@$"Login Success.{ip}");
                var jwtIssuer = _config.GetSection("Jwt:Issuer").Get<string>();
                //var jwtKey = _config.GetSection("Jwt:Key").Get<string>();
                var jwtSignKey = _config.GetSection("Jwt:SignKey").Get<string>();
                var c = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Role, Roles.Users)
                    });
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = c,
                    Issuer = jwtIssuer,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSignKey)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { Code = 200, Message = "", Result = new { token, resp.data } });
            }
            else
            {
                _logger.LogCritical(@$"Login Failed.{ip}");
                return Ok(new { Code = 500, Message = "帳號或密碼錯誤", Result = new { } });
            }
        }


    }
}
