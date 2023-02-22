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
    public class QuotationPromotionsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IQuotationPromotionService _quotationPromotionService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public QuotationPromotionsController(IMapper mapper, IQuotationPromotionService quotationPromotionService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _quotationPromotionService = quotationPromotionService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] QuotationPromotionPaged val)
        {
            var result = await _quotationPromotionService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemovePromotion(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _quotationPromotionService.RemovePromotion(ids);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}
