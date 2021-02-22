using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TMTDentalAPI.ViewControllers
{
    public class StockPickingController : Controller
    {
        private readonly IStockPickingService _stockPickingService;

        public StockPickingController(IStockPickingService stockPickingService)
        {
            _stockPickingService = stockPickingService;
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _stockPickingService.SearchQuery(x => x.Id == id).Include(x => x.Partner).Include(x => x.PickingType)
                    .Include(x => x.MoveLines)
                    .Include("Company.Partner")
                    .Include(x => x.CreatedBy)
                    .Include("MoveLines.Product")
                    .Include("MoveLines.ProductUOM")
                    .FirstOrDefaultAsync();

            if (res == null)
                return NotFound();

            res.MoveLines = res.MoveLines.OrderBy(x => x.Sequence).ToList();

            return View(res);
        }
    }
}
