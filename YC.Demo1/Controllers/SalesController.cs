using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC.Demo1.Models;

namespace YC.Demo1.Controllers
{
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
        [Route("api/Sales")]
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
                    return Ok(new { Code = 501, Message = "Get data failure.", Result = new { } });
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Get data crash.");
                return Ok(new { Code = 500, Message = "Get data crash.", Result = new { } });
            }
        }

        [HttpPut]
        [Route("api/Sales")]
        public async Task<IActionResult> Put([FromBody] PutSales data)
        {
            try
            {
                var ip = Request.HttpContext.Connection.RemoteIpAddress;
                (bool IsSuccess, int NumOfRowsAffected) resp = await _sales.PutSales(data);
                if (resp.IsSuccess == true)
                {
                    return Ok(new { Code = 200, Message = "", Result = new { resp.NumOfRowsAffected } });
                }
                else
                {
                    return Ok(new { Code = 501, Message = "Put data failure.", Result = new { } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Put data crash.");
                return Ok(new { Code = 500, Message = "Put data crash.", Result = new { } });
            }
        }
        [HttpPatch]
        [Route("api/Sales")]
        public async Task<IActionResult> Patch([FromBody] PutSales data)
        {
            try
            {
                var ip = Request.HttpContext.Connection.RemoteIpAddress;
                (bool IsSuccess, int NumOfRowsAffected) resp = await _sales.PatchSales(data);
                if (resp.IsSuccess == true)
                {
                    return Ok(new { Code = 200, Message = "", Result = new { resp.NumOfRowsAffected } });
                }
                else
                {
                    return Ok(new { Code = 501, Message = "Patch data failure.", Result = new { } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Patch data crash.");
                return Ok(new { Code = 500, Message = "Patch data crash.", Result = new { } });
            }
        }
        [HttpDelete]
        [Route("api/Sales")]
        public async Task<IActionResult> Delete([FromBody] DeleteSales data)
        {
            try
            {
                var ip = Request.HttpContext.Connection.RemoteIpAddress;
                (bool IsSuccess, int NumOfRowsAffected) resp = await _sales.DeleteSales(data);
                if (resp.IsSuccess == true)
                {
                    return Ok(new { Code = 200, Message = "", Result = new { resp.NumOfRowsAffected } });
                }
                else
                {
                    return Ok(new { Code = 501, Message = "Delete data failure.", Result = new { } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete data crash.");
                return Ok(new { Code = 500, Message = "Delete data crash.", Result = new { } });
            }
        }
    }
}
