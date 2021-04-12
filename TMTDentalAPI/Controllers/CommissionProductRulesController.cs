using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionProductRulesController : BaseApiController
    {
        private readonly ICommissionProductRuleService _commissionProductRuleService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IViewRenderService _viewRenderService;

        public CommissionProductRulesController (
            ICommissionProductRuleService commissionProductRuleService, 
            IViewRenderService viewRenderService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork
        )
        {
            _commissionProductRuleService = commissionProductRuleService;
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetForCommission(Guid commissionId)
        {
            var result = await _commissionProductRuleService.GetResultAsync(commissionId);
            return Ok(result);
        }
    }
}
