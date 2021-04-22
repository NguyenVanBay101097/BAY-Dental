﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsAccountsController : BaseApiController
    {
        private readonly ISmsAccountService _smsAccountService;
        private readonly IMapper _mapper;
        public SmsAccountsController(IMapper mapper, ISmsAccountService smsAccountService)
        {
            _smsAccountService = smsAccountService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SmsAccountSave val)
        {
            if (!ModelState.IsValid || val == null) return BadRequest();
            var entity = _mapper.Map<SmsAccount>(val);
            entity.CompanyId = CompanyId;
            entity = await _smsAccountService.CreateAsync(entity);
            var res = _mapper.Map<SmsAccountBasic>(entity);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsAccountSave val)
        {
            var entity = await _smsAccountService.GetByIdAsync(id);
            if (!ModelState.IsValid || entity == null) return BadRequest();
            entity = _mapper.Map(val, entity);
            await _smsAccountService.UpdateAsync(entity);
            var res = _mapper.Map<SmsAccountBasic>(entity);
            return Ok(res);
        }
    }
}
