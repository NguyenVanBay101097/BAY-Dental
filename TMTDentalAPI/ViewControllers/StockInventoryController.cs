using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.ViewControllers
{
    public class StockInventoryController : Controller
    {
        private readonly IStockInventoryService _stockInventoryService;
        private readonly IViewToStringRenderService _viewToStringRenderService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public StockInventoryController(IStockInventoryService stockInventoryService, IViewToStringRenderService viewToStringRenderService, IMapper mapper, IUserService userService)
        {
            _stockInventoryService = stockInventoryService;
            _viewToStringRenderService = viewToStringRenderService;
            _mapper = mapper;
            _userService = userService;
        }

        [PrinterNameFilterAttribute(Name = AppConstants.StockInventoryPaperCode)]
        public async Task<IActionResult> Print(Guid id)
        {
            var res = await _stockInventoryService.GetStockInventoryPrint(id);
            if (res == null)
                return NotFound();

            var viewdata = ViewData.ToDictionary(x => x.Key, x => x.Value);

            var html = await _viewToStringRenderService.RenderViewAsync("StockInventory/Print", res, viewdata);

            return Ok(new PrintData() { html = html });
        }
    }
}
