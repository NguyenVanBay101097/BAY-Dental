using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;

namespace TMTDentalAPI.ViewControllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.PurchaseOrderPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _purchaseOrderService.GetPrint(id);
            if (res == null) return NotFound();

            return View(res);
        }
    }
}
