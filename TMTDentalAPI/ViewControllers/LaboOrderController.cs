using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TMTDentalAPI.ViewControllers
{
    public class LaboOrderController : Controller
    {
        private readonly ILaboOrderService _laboOrderService;

        public LaboOrderController(ILaboOrderService laboOrderService)
        {
            _laboOrderService = laboOrderService;
        }

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

            return View(res);
        }
    }
}
