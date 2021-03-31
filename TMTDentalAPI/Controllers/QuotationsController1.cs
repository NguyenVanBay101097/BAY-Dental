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
    public class QuotationsController : BaseApiController
    {
        private readonly IQuotationService _quotationService;
        public QuotationsController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] QuotationPaged val)
        {
            var res = await _quotationService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _quotationService.GetByIdAsync(id);
            if (model == null || !ModelState.IsValid)
                return BadRequest();

            var res = await _quotationService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuotationSave val)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, QuotationSave val)
        {
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _quotationService.GetByIdAsync(id);
            if (model == null)
                return NotFound();
            await _quotationService.DeleteAsync(model);
            return NoContent();
        }
    }
}
