using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YC.Demo1.Configs;
using YC.Demo1.Helpers;
using YC.Demo1.Models;

namespace YC.Demo1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ILogger<WeatherForecastController> _logger;

        public LoginController(ILogger<WeatherForecastController> logger, IConfiguration configuratio)
        {
            _logger = logger;
            _config = configuratio;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody]Login login) 
        {
            //if (ValidateUser(login))
            if (login.UserName.Equals("abcd"))
            {
                var jwtIssuer = _config.GetSection("Jwt:Issuer").Get<string>();
                var jwtKey = _config.GetSection("Jwt:Key").Get<string>();
                var jwtSignKey = _config.GetSection("Jwt:SignKey").Get<string>();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Role, Roles.Users)
                    }),
                    Issuer = jwtIssuer,
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSignKey)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
