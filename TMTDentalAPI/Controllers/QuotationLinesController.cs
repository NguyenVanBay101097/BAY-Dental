using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationLinesController : BaseApiController
    {
        private readonly IQuotationLineService _quotationLineService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public QuotationLinesController(IQuotationLineService quotationLineService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork)
        {
            _quotationLineService = quotationLineService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyDiscountOnQuotationLine(ApplyDiscountViewModel val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _quotationLineService.ApplyDiscountOnQuotationLine(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPromotionUsageCode(ApplyPromotionUsageCode val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _quotationLineService.ApplyPromotionUsageCodeOnQuotationLine(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPromotion(ApplyPromotionRequest val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _quotationLineService.ApplyPromotionOnQuotationLine(val);
            _unitOfWork.Commit();
            return Ok(res);
        }
    }
}
