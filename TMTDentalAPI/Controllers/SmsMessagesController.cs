using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Hangfire;
using Infrastructure.HangfireJobService;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using TMTDentalAPI.JobFilters;
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
        private readonly AppTenant _tenant;


        public SmsMessagesController(ITenant<AppTenant> tenant, IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsMessageService smsMessageService)
        {
            _smsMessageService = smsMessageService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
            _tenant = tenant?.Value;
        }

        [HttpGet]
        [CheckAccess(Actions = "SMS.Message.Read")]
        public async Task<IActionResult> GetPaged([FromQuery] SmsMessagePaged val)
        {
            var res = await _smsMessageService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "SMS.Message.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsMessageService.SearchQuery().Where(x => x.Id == id).FirstOrDefaultAsync();
            return Ok(_mapper.Map<SmsMessageDisplay>(res));
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> ActionCreateReminder(SmsMessageSave val)
        {
            if (!ModelState.IsValid || val == null) return BadRequest();
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsMessageService.ActionCreateReminder(val);
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "SMS.Message.Create")]
        public async Task<IActionResult> CreateAsync(SmsMessageSave val)
        {
            if (!ModelState.IsValid || val == null) return BadRequest();
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsMessageService.CreateAsync(val);
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "SMS.Message.Update")]
        public async Task<IActionResult> ActionSendSms(Guid id)
        {
            var entity = await _smsMessageService.SearchQuery().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsMessageService.ActionSend(id);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SMS.Message.Update")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsMessageService.ActionCancel(ids);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }

        [HttpGet("[action]")]
        public IActionResult ActionStartJobAutomatic()
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{hostName}_Send_Sms_message_detail";
            RecurringJob.AddOrUpdate<ISmsMessageJobService>(jobId, x => x.RunJobFindSmsMessage(hostName, CompanyId), $"*/30 * * * *", TimeZoneInfo.Local);

            return NoContent();
        }

        [HttpGet("[action]")]
        public IActionResult ActionStopJobAutomatic()
        {
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{hostName}_Send_Sms_message_detail";
            RecurringJob.RemoveIfExists(jobId);
            return NoContent();
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "SMS.Message.Update")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsAccountSave val)
        {
            var entity = await _smsMessageService.SearchQuery().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (!ModelState.IsValid || entity == null) return BadRequest();
            entity = _mapper.Map(val, entity);
            entity.CompanyId = CompanyId;
            await _smsMessageService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _smsMessageService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            await _smsMessageService.DeleteAsync(entity);
            return NoContent();
        }
    }
}
