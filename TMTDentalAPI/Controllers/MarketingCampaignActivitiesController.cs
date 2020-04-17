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
    public class MarketingCampaignActivitiesController : BaseApiController
    {
        private readonly IMarketingCampaignActivityService _marketingCampaignActivityService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public MarketingCampaignActivitiesController(IMarketingCampaignActivityService marketingCampaignActivityService,
            IMapper mapper, IUserService userService)
        {
            _marketingCampaignActivityService = marketingCampaignActivityService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _marketingCampaignActivityService.GetActivityDisplay(id);
            return Ok(res);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateActivity(MarketingCampaignActivitySave val)
        {
            var activity = await _marketingCampaignActivityService.CreateActivity(val);
            return Ok(activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(Guid id ,MarketingCampaignActivitySave val)
        {
            await _marketingCampaignActivityService.UpdateActivity(id,val);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveActivity(Guid id)
        {
                 
            await _marketingCampaignActivityService.RemoveActivity(new List<Guid>() { id });
            return NoContent();
        }
        

    }
}