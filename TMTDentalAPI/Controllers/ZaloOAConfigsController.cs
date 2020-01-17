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
    public class ZaloOAConfigsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IZaloOAConfigService _zaloOAConfigService;

        public ZaloOAConfigsController(IMapper mapper, IZaloOAConfigService zaloOAConfigService)
        {
            _mapper = mapper;
            _zaloOAConfigService = zaloOAConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var config = await _zaloOAConfigService.SearchQuery().FirstOrDefaultAsync();
            return Ok(_mapper.Map<ZaloOAConfigBasic>(config));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ZaloOAConfigSave val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();
            var config = await _zaloOAConfigService.SaveConfig(val);
            var basic = _mapper.Map<ZaloOAConfigBasic>(config);
            return Ok(basic);
        }

        [HttpDelete()]
        public async Task<IActionResult> Remove()
        {
            var config = await _zaloOAConfigService.SearchQuery().FirstOrDefaultAsync();
            if (config != null)
                await _zaloOAConfigService.DeleteAsync(config);
            return NoContent();
        }
    }
}