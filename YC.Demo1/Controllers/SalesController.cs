using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC.Demo1.Models;

namespace YC.Demo1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SalesController> _logger;
        private readonly ISalesRepository _sales;

        public SalesController(
            ILogger<SalesController> logger,
            IConfiguration configuratio,
            ISalesRepository sales)
        {
            _logger = logger;
            _config = configuratio;
            _sales = sales;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                var ip = Request.HttpContext.Connection.RemoteIpAddress;
                (bool IsSuccess, List<Sales> ListSales, List<Stores> ListStores, List<Titles> ListTitles) resp = await _sales.GetSales();
                if (resp.IsSuccess == true)
                {
                    return Ok(new { Code = 200, Message = "", Result = new { resp.ListSales, resp.ListStores, resp.ListTitles } });
                }
                else
                {
                    return Ok(new { Code = 501, Message = "取得資料失敗", Result = new { } });
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "取得資料異常");
                return Ok(new { Code = 500, Message = "取得資料異常", Result = new { } });
            }
        }
    }
}
