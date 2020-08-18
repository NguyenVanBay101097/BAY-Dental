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
    public class CommissionReportsController : BaseApiController
    {
        private readonly ICommissionReportService _commissionReportService;
        public CommissionReportsController(ICommissionReportService commissionReportService)
        {
            _commissionReportService = commissionReportService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReport(ReportFilterCommission val)
        {
            var result = await _commissionReportService.GetReport(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReportDetail(ReportFilterCommissionDetail val)
        {
            var result = await _commissionReportService.GetReportDetail(val);
            return Ok(result);
        }
    }
}