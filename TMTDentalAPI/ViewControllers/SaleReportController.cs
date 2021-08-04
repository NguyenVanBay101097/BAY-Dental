using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class SaleReportController : Controller
    {
        private readonly ISaleReportService _saleReportService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public SaleReportController(ISaleReportService saleReportService ,IViewToStringRenderService viewToStringRenderService)
        {
            _saleReportService = saleReportService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.SaleReport)]
        public async Task<IActionResult> ServiceReportByServicePrint([FromQuery] ServiceReportReq val)
        {
            var res = await _saleReportService.ServiceReportByServicePrint(val);
            return View("ServiceReportPrint", res);
        }

        [PrinterNameFilterAttribute(Name = AppConstants.SaleReport)]
        public async Task<IActionResult> ServiceReportByTimePrint([FromQuery] ServiceReportReq val)
        {
            var res = await _saleReportService.ServiceReportByTimePrint(val);
            return View("ServiceReportPrint", res);
        }
    }
}
