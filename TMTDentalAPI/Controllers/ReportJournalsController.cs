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
    public class ReportJournalsController : BaseApiController
    {
        private readonly IReportJournalService _reportJournalService;

        public ReportJournalsController(IReportJournalService reportJournalService)
        {
            _reportJournalService = reportJournalService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCashBankReport(ReportCashBankJournalSearch val)
        {
            var res = await _reportJournalService.GetCashBankReportValues(val);
            return Ok(res);
        }
    }
}