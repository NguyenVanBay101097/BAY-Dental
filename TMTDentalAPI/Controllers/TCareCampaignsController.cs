using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TCareCampaignsController : BaseApiController
    {
        private readonly ITCareCampaignService _campaignService;
        private readonly IMapper _mapper;

        public TCareCampaignsController(ITCareCampaignService campaignService, IMapper mapper)
        {
            _campaignService = campaignService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]TCareCampaignPaged val)
        {
            var result = await _campaignService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> NameCreate(TCareCampaignNameCreateVM val)
        {
            var campaign = await _campaignService.NameCreate(val);
            var basic = _mapper.Map<TCareCampaignBasic>(campaign);
            return Ok(basic);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            var res = _mapper.Map<TCareCampaignBasic>(campaign);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TCareCampaignNameCreateVM val)
        {
            var campain = await _campaignService.GetByIdAsync(id);
            if (campain == null)
            {
                return BadRequest();
            }
            campain.Name = val.Name;
            campain.GraphXml = val.GraphXml;
            await _campaignService.UpdateAsync(campain);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            if (campaign == null)
                return BadRequest();
            await _campaignService.DeleteAsync(campaign);
            return NoContent();
        }
    }
}