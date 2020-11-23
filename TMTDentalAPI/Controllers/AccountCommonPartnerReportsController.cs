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

        [HttpPost("GetSummaryPartner")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetSummaryPartner(AccountCommonPartnerReportSearchV2 val)
        {
            var res = await _reportService.ReportSumaryPartner(val);
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

        [HttpPost("GetSalaryReportEmployee")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetSalaryReportEmployee(AccountCommonPartnerReportSearch val)
        {
            var res = await _reportService.ReportSalaryEmployee(val);
            return Ok(res);
        }
    }
}