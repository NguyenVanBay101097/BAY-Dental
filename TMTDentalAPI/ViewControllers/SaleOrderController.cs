using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.ViewControllers
{
    public class SaleOrderController : Controller
    {
        private readonly ISaleOrderService _saleOrderService;

        public SaleOrderController(ISaleOrderService saleOrderService)
        {
            _saleOrderService = saleOrderService;
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var order = await _saleOrderService.GetPrint(id);
            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
