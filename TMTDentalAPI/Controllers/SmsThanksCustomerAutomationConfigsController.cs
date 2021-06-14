﻿using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
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

        [HttpGet]
        [CheckAccess(Actions = "SMS.Config.Read")]
        public async Task<IActionResult> Get()
        {
            var res = await _smsConfigService.GetByCompany();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "SMS.Config.Create")]
        public async Task<IActionResult> SaveConfig(SmsThanksCustomerAutomationConfigSave val)
        {
            var entity = await _smsConfigService.SearchQuery(x => x.CompanyId == CompanyId).FirstOrDefaultAsync();
            await _unitOfWorkAsync.BeginTransactionAsync();

            if (entity != null)
            {
                entity = _mapper.Map(val, entity);
                await _smsConfigService.UpdateAsync(entity);
            }
            else
            {
                entity = _mapper.Map<SmsThanksCustomerAutomationConfig>(val);
                entity = await _smsConfigService.CreateAsync(entity);
            }
            _unitOfWorkAsync.Commit();


            return NoContent();
        }
    }
}
