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
    public class StockMovesController : ControllerBase
    {
        private readonly IStockMoveService _stockMoveService;
        public StockMovesController(IStockMoveService stockMoveService)
        {
            _stockMoveService = stockMoveService;
        }

        [HttpPost("OnChangeProduct")]
        public async Task<IActionResult> OnChangeProduct(StockMoveOnChangeProduct val)
        {
            var res = await _stockMoveService.OnChangeProduct(val);
            return Ok(res);
        }
    }
}