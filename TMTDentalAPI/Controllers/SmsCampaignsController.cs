using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
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

        public SmsCampaignsController(IMapper mapper, ISmsCampaignService smsCampaignService)
        {
            _smsCampaignService = smsCampaignService;
            _mapper = mapper;
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
            var res = await _smsCampaignService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            return Ok(_mapper.Map<SmsAccountDisplay>(res));
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
            var res = await _smsCampaignService.GetDefaultCampaignAppointmentReminder();
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
