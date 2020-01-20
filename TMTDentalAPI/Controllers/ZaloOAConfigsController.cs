using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Hangfire;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloOAConfigsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IZaloOAConfigService _zaloOAConfigService;
        private readonly AppTenant _tenant;
        private readonly BirthdayMessageJobService _birthdayMessageJobService;

        public ZaloOAConfigsController(IMapper mapper, IZaloOAConfigService zaloOAConfigService,
            ITenant<AppTenant> tenant, BirthdayMessageJobService birthdayMessageJobService)
        {
            _mapper = mapper;
            _zaloOAConfigService = zaloOAConfigService;
            _tenant = tenant?.Value;
            _birthdayMessageJobService = birthdayMessageJobService;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ZaloOAConfigUpdate val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var config = await _zaloOAConfigService.GetByIdAsync(id);
            if (config == null)
                return NotFound();

            config = _mapper.Map(val, config);
            await _zaloOAConfigService.UpdateAsync(config);

            if (val.AutoSendBirthdayMessage)
                RecurringJob.AddOrUpdate($"{_tenant.Hostname}-birthday", () => _birthdayMessageJobService.SendMessage(_tenant.Hostname), "30 9 * * * *");
            else
                RecurringJob.RemoveIfExists($"{_tenant.Hostname}-birthday");

            return NoContent();
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