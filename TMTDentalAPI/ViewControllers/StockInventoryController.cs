using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.ViewControllers
{
    public class StockInventoryController : Controller
    {
        private readonly IStockInventoryService _stockInventoryService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public StockInventoryController(IStockInventoryService stockInventoryService, IMapper mapper, IUserService userService)
        {
            _stockInventoryService = stockInventoryService;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var inventory = await _stockInventoryService.GetStockInventoryPrint(id);
            if (inventory == null)
                return NotFound();

            return View(inventory);
        }
    }
}
