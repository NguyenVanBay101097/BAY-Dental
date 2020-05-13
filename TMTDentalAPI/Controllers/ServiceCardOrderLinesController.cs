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
    public class ServiceCardOrderLinesController : BaseApiController
    {
        private readonly IServiceCardOrderLineService _saleLineService;

        public ServiceCardOrderLinesController(IServiceCardOrderLineService saleLineService)
        {
            _saleLineService = saleLineService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangeProduct(ServiceCardOrderLineOnChangeCardType val)
        {
            var res = await _saleLineService.OnChangeCardType(val);
            return Ok(res);
        }
    }
}