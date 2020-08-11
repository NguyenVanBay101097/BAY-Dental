using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionReportsController : BaseApiController
    {
        private readonly ICommissionReportService _commissionReportService;
        public CommissionReportsController(ICommissionReportService commissionReportService)
        {
            _commissionReportService = commissionReportService;
        }

        [HttpPost]
        public async Task<IActionResult> GetReport(ReportFilterCommission val)
        {
            var result = await _commissionReportService.GetReport(val);
            return Ok(result);
        }
    }
}