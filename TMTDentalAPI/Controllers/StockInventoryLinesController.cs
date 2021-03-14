using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockInventoryLinesController : BaseApiController
    {
        private readonly IStockInventoryLineService _stockInventoryLineService;
        public StockInventoryLinesController(IStockInventoryLineService stockInventoryLineService)
        {
            _stockInventoryLineService = stockInventoryLineService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangeCreateLine(StockInventoryLineOnChangeCreateLine val)
        {
            var res = await _stockInventoryLineService.OnChangeCreateLine(val);
            return Ok(res);
        }
    }
}