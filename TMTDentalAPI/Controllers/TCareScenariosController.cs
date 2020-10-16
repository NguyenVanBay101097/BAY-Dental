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
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TCareScenariosController : ControllerBase
    {
        private readonly ITCareScenarioService _scenarioService;
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        private readonly ITCareJobService _tcareJobService;

        public TCareScenariosController(ITCareScenarioService scenarioService, IMapper mapper,
            ITenant<AppTenant> tenant, ITCareJobService tcareJobService)
        {
            _scenarioService = scenarioService;
            _mapper = mapper;
            _tenant = tenant?.Value;
            _tcareJobService = tcareJobService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]TCareScenarioPaged paged)
        {
            var res = await _scenarioService.GetPagedResultAsync(paged);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _scenarioService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Autocomplete(TCareScenarioPaged val)
        {
            var res = await _scenarioService.GetAutocompleteAsync(val);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TCareScenarioSave val)
        {
            var model = new TCareScenario();
            model.Name = val.Name;
            model.ChannelSocialId = val.ChannelSocialId;
            var res = await _scenarioService.CreateAsync(model);
            var display = _mapper.Map<TCareScenarioDisplay>(res);
            return Ok(display);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, TCareScenarioSave val)
        {
            var model = await _scenarioService.GetByIdAsync(id);
            model.Name = val.Name;
            model.ChannelSocialId = val.ChannelSocialId;
            await _scenarioService.UpdateAsync(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var model = await _scenarioService.GetByIdAsync(id);
            await _scenarioService.DeleteAsync(model);
            return NoContent();
        }

        [HttpGet("[action]")]
        public IActionResult AddJob()
        {
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{tenant}-tcare-scenario-Job";
            RecurringJob.RemoveIfExists(jobId);
            var now = DateTime.Now.AddMinutes(1);
            RecurringJob.AddOrUpdate(jobId, () => _tcareJobService.TCareTakeMessage(tenant), $"40 18 * * *", TimeZoneInfo.Local);
            return NoContent();
        }

        /// <summary>
        /// config : Cron Run once a "*/{Minute} */{Hour} */{day of month} */{month} *{day of week}"
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public IActionResult AddJob2()
        {
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{tenant}-tcare-scenario-Job2";
            RecurringJob.RemoveIfExists(jobId);
            RecurringJob.AddOrUpdate(jobId, () => _tcareJobService.RunJob2Messages(tenant), $"* */1 * * *", TimeZoneInfo.Local);
            return NoContent();
        }
    }
}