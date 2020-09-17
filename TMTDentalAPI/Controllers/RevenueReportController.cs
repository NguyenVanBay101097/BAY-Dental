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
    public class RevenueReportController : BaseApiController
    {
        private readonly IRevenueReportService _revenueReportService;
        public RevenueReportController(IRevenueReportService revenueReportService)
        {
            _revenueReportService = revenueReportService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> Get([FromQuery]RevenueReportSearch val)
        {
            var result = await _revenueReportService.GetReport(val);
            return Ok(result);
        }
    }
}