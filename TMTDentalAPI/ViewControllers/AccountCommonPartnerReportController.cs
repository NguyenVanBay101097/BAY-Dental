using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IActionResult PrintReportPartnerDebit([FromBody]ReportPartnerDebitReq val)
        {
            var result = _service.PrintReportPartnerDebit(val);
            return View(result);
        }
    }
}
