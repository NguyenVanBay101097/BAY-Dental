using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class SmsBirthdayAutomationConfigsController : BaseApiController
    {
        private readonly ISmsBirthdayAutomationConfigService _smsConfigService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public SmsBirthdayAutomationConfigsController(IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsBirthdayAutomationConfigService smsConfigService)
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
        public async Task<IActionResult> CreateAsync(SmsBirthdayAutomationConfigSave val)
        {
            var entity = _mapper.Map<SmsBirthdayAutomationConfig>(val);
            await _unitOfWorkAsync.BeginTransactionAsync();
            entity = await _smsConfigService.CreateAsync(entity);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "SMS.Config.Update")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsBirthdayAutomationConfigSave val)
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
