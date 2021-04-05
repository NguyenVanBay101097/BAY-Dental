using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class StockPickingController : Controller
    {
        private readonly IStockPickingService _stockPickingService;
        private readonly IViewToStringRenderService _viewToStringRenderService;

        public StockPickingController(IStockPickingService stockPickingService, IViewToStringRenderService viewToStringRenderService)
        {
            _stockPickingService = stockPickingService;
            _viewToStringRenderService = viewToStringRenderService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.StockPickingPaperCode)]
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
