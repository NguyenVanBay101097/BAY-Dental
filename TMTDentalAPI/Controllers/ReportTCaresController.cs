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
    public class ReportTCaresController : BaseApiController
    {
        private readonly ITCareReportService _tCareReport;

        public ReportTCaresController(ITCareReportService tCareReport)
        {
            _tCareReport = tCareReport;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReport(TCareScenarioFilterReport val)
        {
            var res = await _tCareReport.GetReportTCare(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReportDetail(TCareReports val)
        {
            var res = await _tCareReport.GetReportTCareDetail(val);
            return Ok(res);
        }
    }
}