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
    public class PartnerOldNewReportsController : ControllerBase
    {
        private IPartnerOldNewReportService _partnerOldNewReportService;
        public PartnerOldNewReportsController(IPartnerOldNewReportService partnerOldNewReportService)
        {
            _partnerOldNewReportService = partnerOldNewReportService;
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.PartnerOldNew")]
        public async Task<IActionResult> GetPartnerOldNewReport(PartnerOldNewReportSearch val)
        {
            var res = await _partnerOldNewReportService.GetPartnerOldNewReport(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.PartnerOldNew")]
        public async Task<IActionResult> GetSumaryPartnerOldNewReport(PartnerOldNewReportSearch val)
        {
            var res = await _partnerOldNewReportService.GetSumaryPartnerOldNewReport(val);
            return Ok(res);
        }
    }
}
