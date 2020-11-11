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
    public class AccountCommonPartnerReportsController : BaseApiController
    {
        private readonly IAccountCommonPartnerReportService _reportService;
        public AccountCommonPartnerReportsController(IAccountCommonPartnerReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("GetSummaryByPartner/{id}")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetSummaryByPartner(Guid id)
        {
            var res = await _reportService.ReportSumaryByPartner(id);
            return Ok(res);
        }

        [HttpPost("GetSummary")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetSummary(AccountCommonPartnerReportSearch val)
        {
            var res = await _reportService.ReportSummary(val);
            return Ok(res);
        }

        [HttpPost("GetDetail")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetDetail(AccountCommonPartnerReportItem val)
        {
            var res = await _reportService.ReportDetail(val);
            return Ok(res);
        }
    }
}