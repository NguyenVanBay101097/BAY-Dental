using ApplicationCore.Constants;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;

namespace TMTDentalAPI.ViewControllers
{
    public class SaleOrderPaymentController : Controller
    {
        private readonly ISaleOrderPaymentService _saleOrderPaymentService;

        public SaleOrderPaymentController(ISaleOrderPaymentService saleOrderPaymentService)
        {
            _saleOrderPaymentService = saleOrderPaymentService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.SaleOrderPaymentPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _saleOrderPaymentService.GetPrint(id);
            if (res == null) return NotFound();

            return View(res);
        }
    }
}
