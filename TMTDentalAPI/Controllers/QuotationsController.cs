using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
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
        private readonly IViewRenderService _viewRenderService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public QuotationsController(IViewRenderService viewRenderService, IUnitOfWorkAsync unitOfWork, IQuotationService quotationService)
        {
            _quotationService = quotationService;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
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

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPromotionUsageCode(ApplyPromotionUsageCode val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _quotationService.ApplyPromotionUsageCode(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyDiscountOnOrder(ApplyDiscountViewModel val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _quotationService.ApplyDiscountOnQuotation(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPromotion(ApplyPromotionRequest val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _quotationService.ApplyPromotionOnQuotation(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> CreateSaleOrderByQuotation(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _quotationService.CreateSaleOrderByQuotation(id);
            _unitOfWork.Commit();
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

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Print(Guid id)
        {
            var quotation = await _quotationService.Print(id);
            if (quotation == null)
            {
                return NotFound();
            }
            var html = _viewRenderService.Render<QuotationPrintVM>("Quotation/Print", quotation);
            return Ok(new PrintData() { html = html });
        }
    }
}
