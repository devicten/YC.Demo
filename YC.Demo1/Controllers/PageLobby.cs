using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YC.Demo1.Controllers
{
    [Route("Lobby")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PageLobby : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
