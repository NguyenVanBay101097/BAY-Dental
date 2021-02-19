using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.ViewControllers
{
    public class MedicineOrderController : Controller
    {
        private readonly IMedicineOrderService _medicineOrderService;

        public MedicineOrderController(IMedicineOrderService medicineOrderService)
        {
            _medicineOrderService = medicineOrderService;
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _medicineOrderService.GetPrint(id);
            if (res == null)
                return NotFound();

            return View(res);
        }
    }
}
