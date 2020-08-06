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
    public class CommissionsController : BaseApiController
    {
        private readonly ICommissionService _commissionService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public CommissionsController(ICommissionService commissionService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _commissionService = commissionService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]CommissionPaged val)
        {
            var result = await _commissionService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _commissionService.GetCommissionForDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommissionDisplay val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var commission = await _commissionService.CreateCommission(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<CommissionBasic>(commission);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CommissionDisplay val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _commissionService.UpdateCommission(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var commission = await _commissionService.GetByIdAsync(id);
            if (commission == null)
                return NotFound();

            await _commissionService.DeleteAsync(commission);

            return NoContent();
        }
    }
}