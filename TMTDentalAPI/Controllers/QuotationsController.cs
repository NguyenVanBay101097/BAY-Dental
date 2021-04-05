using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
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
        private readonly IUnitOfWorkAsync _unitOfWork;
        public QuotationsController(IUnitOfWorkAsync unitOfWork, IQuotationService quotationService)
        {
            _quotationService = quotationService;
            _unitOfWork = unitOfWork;
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

        [HttpGet("[action]/{partnerId}")]
        public async Task<IActionResult> GetDefault(Guid partnerId)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var res = await _quotationService.GetDefault(partnerId);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuotationSave val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _quotationService.CreateAsync(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, QuotationSave val)
        {
            var model = await _quotationService.GetByIdAsync(id);
            if (model == null || !ModelState.IsValid)
                return NotFound();
            await _unitOfWork.BeginTransactionAsync();
            await _quotationService.UpdateAsync(id, val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> CreateSaleOrderByQuotation(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var res = await _quotationService.CreateSaleOrderByQuotation(id);
            return Ok(res);
        }

        [HttpDelete("{id}")]
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
