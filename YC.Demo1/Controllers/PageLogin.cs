using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YC.Demo1.Configs;
using YC.Demo1.Models;

namespace YC.Demo1.Controllers
{

    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PageLogin : Controller
    {
        [Route("Login")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Login/WebApp")]
        public async Task<IActionResult> WebApp([FromForm] LoginWebApp data)
        {
            Response.Cookies.Append("BearToken", data.token);
            var c = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, Roles.Users) });
            return RedirectToAction("Index", "PageLobby", null);
        }

    }
}
