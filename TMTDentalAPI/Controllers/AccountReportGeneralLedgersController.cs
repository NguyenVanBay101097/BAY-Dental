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
    public class AccountReportGeneralLedgersController : BaseApiController
    {
        private readonly IAccountReportGeneralLedgerService _reportGeneralLedgerService;

        public AccountReportGeneralLedgersController(IAccountReportGeneralLedgerService reportGeneralLedgerService)
        {
            _reportGeneralLedgerService = reportGeneralLedgerService;
        }

        [HttpPost("[action]")]
        public IActionResult GetCashBankReport(ReportCashBankGeneralLedger val)
        {
            var res = _reportGeneralLedgerService.GetCashBankReportValues(val);
            return Ok(res);
        }
    }
}