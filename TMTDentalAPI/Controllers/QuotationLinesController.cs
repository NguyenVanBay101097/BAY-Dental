using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationLinesController : ControllerBase
    {
        private readonly IQuotationLineService _quotationLineService;
        public QuotationLinesController(IQuotationLineService quotationLineService)
        {
            _quotationLineService = quotationLineService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuotationLineDisplayDetail(Guid id)
        {
            var res = await _quotationLineService.GetQuotationLineDisplayDetail(id);
            return Ok(res);
        }
    }
}
