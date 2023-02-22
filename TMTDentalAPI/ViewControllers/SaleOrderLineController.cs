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
    public class SaleOrderLineController : Controller
    {
        private readonly ISaleOrderLineService _saleOrderLineService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public SaleOrderLineController(ISaleOrderLineService saleOrderLineService, IViewToStringRenderService viewToStringRenderService)
        {
            _saleOrderLineService = saleOrderLineService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.SaleOrderLineCode)]
        public async Task<IActionResult> SaleReportPrint([FromQuery] SaleOrderLinesPaged val)
        {
            var res = await _saleOrderLineService.SaleReportPrint(val);
            return View(res);
        }
    }
}
