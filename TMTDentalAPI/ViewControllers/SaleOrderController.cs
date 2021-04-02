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
    public class SaleOrderController : Controller
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public SaleOrderController(ISaleOrderService saleOrderService, IViewToStringRenderService viewToStringRenderService)
        {
            _saleOrderService = saleOrderService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        [PrinterNameFilterAttribute(Name = AppConstants.SaleOrderPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var saleOrder = await _saleOrderService.GetPrint(id);
            if (saleOrder == null)
                return NotFound();

            return View(saleOrder);

        }
    }
}
