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
    public class AccountCommonPartnerReportsController : BaseApiController
    {
        private readonly IAccountCommonPartnerReportService _reportService;
        public AccountCommonPartnerReportsController(IAccountCommonPartnerReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("GetSummary")]
        public async Task<IActionResult> GetSummary(AccountCommonPartnerReportSearch val)
        {
            var res = await _reportService.ReportSummary(val);
            return Ok(res);
        }

        [HttpPost("GetDetail")]
        public async Task<IActionResult> GetDetail(AccountCommonPartnerReportItem val)
        {
            var res = await _reportService.ReportDetail(val);
            return Ok(res);
        }
    }
}