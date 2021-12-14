using ApplicationCore.Constants;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class ResInsuranceController : Controller
    {
        private readonly IResInsuranceReportService _service;
        public ResInsuranceController(IResInsuranceReportService service)
        {
            _service = service;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.ReportInsuranceDebt)]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> PrintGetSummary([FromBody] InsuranceReportFilter val)
        {
            var result = await _service.ReportSummaryPrint(val);
            return View(result);
        }
    }
}
