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
    public class MarketingCampaignsController : BaseApiController
    {
        private readonly IMarketingCampaignService _marketingCampaignService;
        private readonly IMapper _mapper;

        public MarketingCampaignsController(IMarketingCampaignService marketingCampaignService,
            IMapper mapper)
        {
            _marketingCampaignService = marketingCampaignService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]MarketingCampaignPaged val)
        {
            var result = await _marketingCampaignService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var campaign = await _marketingCampaignService.SearchQuery(x => x.Id == id)
                .Include(x => x.Activities).FirstOrDefaultAsync();
            if (campaign == null)
                return NotFound();

            var res = _mapper.Map<MarketingCampaignDisplay>(campaign);
            res.Activities = res.Activities.OrderBy(x => x.Sequence);

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MarketingCampaignSave val)
        {
            var campaign = await _marketingCampaignService.CreateCampaign(val);

            var basic = _mapper.Map<MarketingCampaignBasic>(campaign);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, MarketingCampaignSave val)
        {
            await _marketingCampaignService.UpdateCampaign(id, val);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var campaign = await _marketingCampaignService.GetByIdAsync(id);
            if (campaign == null)
                return NotFound();
            await _marketingCampaignService.DeleteAsync(campaign);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionStartCampaign(IEnumerable<Guid> ids)
        {
            await _marketingCampaignService.ActionStartCampaign(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionStopCampaign(IEnumerable<Guid> ids)
        {
            await _marketingCampaignService.ActionStopCampaign(ids);
            return NoContent();
        }
    }
}