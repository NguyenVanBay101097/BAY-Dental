using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleReportsController : BaseApiController
    {
        private readonly ISaleReportService _saleReportService;

        public SaleReportsController(ISaleReportService saleReportService)
        {
            _saleReportService = saleReportService;
        }

        [HttpGet("GetTopService")]
        public async Task<IActionResult> GetTopService([FromQuery]SaleReportTopServicesFilter val)
        {
            var res = await _saleReportService.GetTopServices(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReport(SaleReportSearch val)
        {
            var res = await _saleReportService.GetReport(val);
            return Ok(res);
        }
    }
}