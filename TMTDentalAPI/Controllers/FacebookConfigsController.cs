using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class FacebookConfigsController : BaseApiController
    {
        private readonly IFacebookConfigService _facebookConfigService;
        private readonly IMapper _mapper;

        public FacebookConfigsController(IFacebookConfigService facebookConfigService,
            IMapper mapper)
        {
            _facebookConfigService = facebookConfigService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var config = await _facebookConfigService.SearchQuery(x => x.Id == id)
                .Include(x => x.ConfigPages).FirstOrDefaultAsync();
            if (config == null)
                return NotFound();

            return Ok(_mapper.Map<FacebookConfigDisplay>(config));
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacebookConfigSave val)
        {
            var config = await _facebookConfigService.CreateConfig(val);
            var basic = _mapper.Map<FacebookConfigBasic>(config);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Connect(FacebookConfigConnectSave val)
        {
            var config = await _facebookConfigService.CreateConfigConnect(val);
            var basic = _mapper.Map<FacebookConfigBasic>(config);
            return Ok(basic);
        }
    }
}