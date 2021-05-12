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
    public class SaleOrderPromotionsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderPromotionService _orderPromotionService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SaleOrderPromotionsController(IMapper mapper, ISaleOrderPromotionService orderPromotionService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _orderPromotionService = orderPromotionService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPromotionPaged val)
        {
            var result = await _orderPromotionService.GetPagedResultAsync(val);
            return Ok(result);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetHistoryPromotionByCouponProgram([FromQuery] HistoryPromotionRequest val)
        {
            var result = await _orderPromotionService.GetHistoryPromotionResult(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemovePromotion(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _orderPromotionService.RemovePromotion(ids);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}
