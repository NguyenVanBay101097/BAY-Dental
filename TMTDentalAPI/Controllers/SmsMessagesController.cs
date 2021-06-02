﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsMessagesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISmsMessageService _smsMessageService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public SmsMessagesController(IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsMessageService smsMessageService)
        {
            _smsMessageService = smsMessageService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SmsMessageSave val)
        {
            if (!ModelState.IsValid || val == null) return BadRequest();
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsMessageService.CreateAsync(val);
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> ActionSendSms(Guid id)
        {
            var entity = await _smsMessageService.SearchQuery(x => x.Id == id)
                .Include(x => x.SmsMessageAppointmentRels).ThenInclude(x => x.Appointment)
                .Include(x => x.SmsMessageSaleOrderLineRels).ThenInclude(x => x.SaleOrderLine)
                .Include(x => x.SmsMessageSaleOrderRels).ThenInclude(x => x.SaleOrder)
                .Include(x => x.SmsAccount)
                .Include(x => x.SmsMessagePartnerRels).FirstOrDefaultAsync();
            if (entity == null) return NotFound();
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsMessageService.ActionSendSMSMessage(entity);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsMessageService.ActionCancel(ids);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }

        [HttpGet("[action]/{orderId}")]
        public async Task<IActionResult> SetupSendSmsOrderAutomatic(Guid orderId)
        {
            if (!ModelState.IsValid) return BadRequest();
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsMessageService.SetupSendSmsOrderAutomatic(orderId);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsMessageService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            return Ok(_mapper.Map<SmsMessageDisplay>(res));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] SmsMessagePaged val)
        {
            var res = await _smsMessageService.GetPaged(val);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsAccountSave val)
        {
            var entity = await _smsMessageService.GetByIdAsync(id);
            if (!ModelState.IsValid || entity == null) return BadRequest();
            entity = _mapper.Map(val, entity);
            entity.CompanyId = CompanyId;
            await _smsMessageService.UpdateAsync(entity);
            var res = _mapper.Map<SmsMessageDisplay>(entity);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _smsMessageService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return NoContent();
        }
    }
}
