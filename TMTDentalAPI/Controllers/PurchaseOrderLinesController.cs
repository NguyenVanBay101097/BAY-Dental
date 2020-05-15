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
    public class PurchaseOrderLinesController : BaseApiController
    {
        private readonly IPurchaseOrderLineService _purchaseLineService;
        public PurchaseOrderLinesController(IPurchaseOrderLineService purchaseLineService)
        {
            _purchaseLineService = purchaseLineService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangeProduct(PurchaseOrderLineOnChangeProduct val)
        {
            var res = await _purchaseLineService.OnChangeProduct(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangeUOM(PurchaseOrderLineOnChangeUOM val)
        {
            var res = await _purchaseLineService.OnChangeUOM(val);
            return Ok(res);
        }
    }
}