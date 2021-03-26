using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class LaboOrderController : Controller
    {
        private readonly ILaboOrderService _laboOrderService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public LaboOrderController(ILaboOrderService laboOrderService, IViewToStringRenderService viewToStringRenderService)
        {
            _laboOrderService = laboOrderService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        [PrinterNameFilterAttribute(Name = "LaboOrderPaperFormat")]
        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _laboOrderService.SearchQuery(x => x.Id == id)
              .Include(x => x.Company.Partner)
              .Include(x => x.Product)
              .Include(x => x.LaboBridge)
              .Include(x => x.LaboBiteJoint)
              .Include(x => x.LaboFinishLine)
              .Include(x => x.SaleOrderLine.Product)
              .Include(x => x.SaleOrderLine.Order)
              .Include(x => x.SaleOrderLine.Employee)
              .Include(x => x.LaboOrderProductRel).ThenInclude(x => x.Product)
              .Include(x => x.Partner)
              .Include(x => x.Customer)
              .Include("LaboOrderToothRel.Tooth")
              .FirstOrDefaultAsync();
            if (res == null)
                return NotFound();

            var viewdata = ViewData["ConfigPrint"] as ConfigPrintDisplay;

            var html = await _viewToStringRenderService.RenderViewAsync("LaboOrder/Print", res, viewdata);

            return Ok(new PrintData() { html = html });

        }
    }
}
