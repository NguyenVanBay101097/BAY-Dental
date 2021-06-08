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
    public class CommissionSettlementsController : BaseApiController
    {
        private readonly ICommissionSettlementService _commissionSettlementService;
        public CommissionSettlementsController(ICommissionSettlementService commissionSettlementService)
        {
            _commissionSettlementService = commissionSettlementService;
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Commission")]
        public async Task<IActionResult> GetReport(CommissionSettlementReport val)
        {
            var result = await _commissionSettlementService.GetReport(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Commission")]
        public async Task<IActionResult> GetReportDetail(CommissionSettlementDetailReportPar val)
        {
            var result = await _commissionSettlementService.GetReportDetail(val);
            return Ok(result);
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Commission")]
        public async Task<IActionResult> GetSumReport(CommissionSettlementReport val)
        {
            var result = await _commissionSettlementService.GetSumReport(val);
            return Ok();
        }

    }
}
