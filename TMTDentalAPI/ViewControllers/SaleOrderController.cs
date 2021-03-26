using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [PrinterNameFilterAttribute(Name = "SaleOrderPaperFormat")]
        public async Task<IActionResult> Print(Guid id)
        {
            var saleOrder = await _saleOrderService.GetPrint(id);
            if (saleOrder == null)
                return NotFound();

            var viewdata = ViewData["ConfigPrint"] as ConfigPrintDisplay;

            var html = await _viewToStringRenderService.RenderViewAsync("SaleOrder/Print", saleOrder, viewdata);

            return Ok(new PrintData() { html = html });

        }
    }
}
