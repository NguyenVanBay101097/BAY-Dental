using System;
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
    public class SmsCampaignsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISmsCampaignService _smsCampaignService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public SmsCampaignsController(IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsCampaignService smsCampaignService)
        {
            _smsCampaignService = smsCampaignService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SmsCampaignSave val)
        {
            if (!ModelState.IsValid || val == null) return BadRequest();
            var res = await _smsCampaignService.CreateAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsCampaignService.GetDisplay(id);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPaged([FromQuery] SmsCampaignPaged val)
        {
            var res = await _smsCampaignService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDefaultCampaignBirthday()
        {
            var res = await _smsCampaignService.GetDefaultCampaignBirthday();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDefaultCampaignAppointmentReminder()
        {
            await _unitOfWorkAsync.BeginTransactionAsync();
            var res = await _smsCampaignService.GetDefaultCampaignAppointmentReminder();
            _unitOfWorkAsync.Commit();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDefaultThanksCustomer()
        {
            var res = await _smsCampaignService.GetDefaultThanksCustomer();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDefaultCareAfterOrder()
        {
            var res = await _smsCampaignService.GetDefaultCareAfterOrder();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDefaultCampaign()
        {
            var res = await _smsCampaignService.GetDefaultCampaign();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsCampaignSave val)
        {
            var entity = await _smsCampaignService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            await _smsCampaignService.UpdateAsync(id, val);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _smsCampaignService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            await _smsCampaignService.DeleteAsync(entity);
            return NoContent();
        }
    }
}
