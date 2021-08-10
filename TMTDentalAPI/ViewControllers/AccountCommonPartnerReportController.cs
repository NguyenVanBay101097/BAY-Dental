using ApplicationCore.Constants;
using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class AccountCommonPartnerReportController : Controller
    {
        private readonly IAccountCommonPartnerReportService _service;
        public AccountCommonPartnerReportController(IAccountCommonPartnerReportService service)
        {
            _service = service;
        }
        [PrinterNameFilterAttribute(Name = AppConstants.ReportPartnerDebit)]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> PrintReportPartnerDebit([FromBody]ReportPartnerDebitReq val)
        {
            var result = await _service.PrintReportPartnerDebit(val);
            return View(result);
        }
    }
}
