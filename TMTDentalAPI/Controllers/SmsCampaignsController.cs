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
using TMTDentalAPI.JobFilters;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsCampaignsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISmsCampaignService _smsCampaignService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly AppTenant _tenant;

        public SmsCampaignsController(ITenant<AppTenant> tenant, IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsCampaignService smsCampaignService)
        {
            _smsCampaignService = smsCampaignService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
            _tenant = tenant?.Value;
        }



        [HttpGet("[action]")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> GetPaged([FromQuery] SmsCampaignPaged val)
        {
            var res = await _smsCampaignService.GetPaged(val);
            return Ok(res);
        }



        [HttpGet("{id}")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsCampaignService.GetDisplay(id);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> GetDefaultCampaignBirthday()
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsCampaignService.GetDefaultCampaignBirthday();
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> GetDefaultCampaignAppointmentReminder()
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsCampaignService.GetDefaultCampaignAppointmentReminder();
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> GetDefaultThanksCustomer()
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsCampaignService.GetDefaultThanksCustomer();
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> GetDefaultCareAfterOrder()
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsCampaignService.GetDefaultCareAfterOrder();
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SMS.Campaign.Read")]
        public async Task<IActionResult> GetDefaultCampaign()
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsCampaignService.GetDefaultCampaign();
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "SMS.Campaign.Create")]
        public async Task<IActionResult> CreateAsync(SmsCampaignSave val)
        {
            if (!ModelState.IsValid || val == null) return BadRequest();
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsCampaignService.CreateAsync(val);
            _unitOfWorkAsync.Commit();

            return Ok(res);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "SMS.Campaign.Update")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsCampaignSave val)
        {
            var entity = await _smsCampaignService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            entity.CompanyId = CompanyId;
            await _smsCampaignService.UpdateAsync(id, val);
            return NoContent();
        }

        [HttpDelete("{id}")]
        //[CheckAccess(Actions = "SMS.Campaign.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _smsCampaignService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            await _smsCampaignService.DeleteAsync(entity);
            return NoContent();
        }
    }
}
