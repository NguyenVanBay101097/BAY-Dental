using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealRevenueReportController : BaseApiController
    {
        private readonly IRealRevenueReportService _realRevenueReportService;
        public RealRevenueReportController(IRealRevenueReportService realRevenueReportService)
        {
            _realRevenueReportService = realRevenueReportService;
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.RealRevenue")]
        public async Task<IActionResult> GetReport(RealRevenueReportSearch val)
        {
            var res = await _realRevenueReportService.GetReport(val);
            return Ok(res);
        }
    }
}