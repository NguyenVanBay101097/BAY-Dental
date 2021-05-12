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
            var entity = _mapper.Map<SmsCampaign>(val);
            entity.CompanyId = CompanyId;
            entity = await _smsCampaignService.CreateAsync(entity);
            var res = _mapper.Map<SmsAccountBasic>(entity);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsAccountSave val)
        {
            var entity = await _smsCampaignService.GetByIdAsync(id);
            if (!ModelState.IsValid || entity == null) return BadRequest();
            entity = _mapper.Map(val, entity);
            entity.CompanyId = CompanyId;
            await _smsCampaignService.UpdateAsync(entity);
            var res = _mapper.Map<SmsAccountBasic>(entity);
            return Ok(res);
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
