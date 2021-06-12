using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsThanksCustomerAutomationConfigsController : BaseApiController
    {
        private readonly ISmsThanksCustomerAutomationConfigService _smsConfigService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public SmsThanksCustomerAutomationConfigsController(IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsThanksCustomerAutomationConfigService smsConfigService)
        {
            _smsConfigService = smsConfigService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "SMS.Config.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsConfigService.GetByCompany(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "SMS.Config.Create")]
        public async Task<IActionResult> CreateAsync(SmsThanksCustomerAutomationConfigSave val)
        {
            var entity = _mapper.Map<SmsThanksCustomerAutomationConfig>(val);
            await _unitOfWorkAsync.BeginTransactionAsync();
            entity = await _smsConfigService.CreateAsync(entity);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "SMS.Config.Update")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsThanksCustomerAutomationConfigSave val)
        {
            var entity = await _smsConfigService.GetByIdAsync(id);
            if (entity == null || !ModelState.IsValid)
                return NotFound();
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsConfigService.UpdateAsync(id, val);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }
    }
}
