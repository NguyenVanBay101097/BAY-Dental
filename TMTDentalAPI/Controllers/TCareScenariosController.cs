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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TCareScenariosController : ControllerBase
    {
        private readonly ITCareScenarioService _scenarioService;
        private readonly ITCareCampaignService _tcareCampaignService;
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        private readonly ITCareJobService _tcareJobService;

        public TCareScenariosController(ITCareScenarioService scenarioService, IMapper mapper,
            ITenant<AppTenant> tenant, ITCareJobService tcareJobService, ITCareCampaignService tcareCampaignService)
        {
            _scenarioService = scenarioService;
            _mapper = mapper;
            _tenant = tenant?.Value;
            _tcareJobService = tcareJobService;
            _tcareCampaignService = tcareCampaignService;
        }

        [HttpGet]
        [CheckAccess(Actions = "TCare.Scenario.Read")]
        public async Task<IActionResult> Get([FromQuery] TCareScenarioPaged paged)
        {
            var res = await _scenarioService.GetPagedResultAsync(paged);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "TCare.Scenario.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _scenarioService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "TCare.Scenario.Read")]
        public async Task<IActionResult> Autocomplete(TCareScenarioPaged val)
        {
            var res = await _scenarioService.GetAutocompleteAsync(val);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "TCare.Scenario.Create")]
        public async Task<IActionResult> CreateAsync(TCareScenarioSave val)
        {
            var model = _mapper.Map<TCareScenario>(val);
            var res = await _scenarioService.CreateAsync(model);
            var display = _mapper.Map<TCareScenarioDisplay>(res);
            return Ok(display);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "TCare.Scenario.Update")]
        public async Task<IActionResult> UpdateAsync(Guid id, TCareScenarioSave val)
        {
            var model = await _scenarioService.GetByIdAsync(id);
            var res = _mapper.Map(val,model);
            await _scenarioService.UpdateAsync(res);


            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "TCare.Scenario.Delete")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var model = await _scenarioService.GetByIdAsync(id);
            if(model.JobId != null)
            RecurringJob.RemoveIfExists(model.JobId);

            await _scenarioService.DeleteAsync(model);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "TCare.Scenario.Update")]
        public async Task<IActionResult> ActionStart(IEnumerable<Guid> ids)
        {
            await _scenarioService.ActionStart(ids);
            return NoContent();
        }

        //[HttpGet("[action]")]
        //public IActionResult AddJob()
        //{
        //    var tenant = _tenant != null ? _tenant.Hostname : "localhost";
        //    var jobId = $"{tenant}-tcare-campaign-job";
        //    RecurringJob.RemoveIfExists(jobId);
        //    var now = DateTime.Now.AddMinutes(1);
        //    RecurringJob.AddOrUpdate<TCareCampaignJobService>(jobId, x => x.Run(tenant, null), $"{now.Minute} {now.Hour} * * *", TimeZoneInfo.Local);
        //    return NoContent();
        //}

        /// <summary>
        /// config : Cron Run once a "*/{Minute} */{Hour} {day of month} */{month} *{day of week}"
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public IActionResult AddJob2()
        {
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{tenant}-tcare-messaging-job";
            RecurringJob.RemoveIfExists(jobId);
            RecurringJob.AddOrUpdate<TCareMessagingJobService>(jobId, x => x.ProcessQueue(tenant), $"* * * * *", TimeZoneInfo.Local);
            return NoContent();
        }

        [HttpGet("[action]")]
        public IActionResult AddJob3()
        {
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{tenant}-tcare-message-job";
            RecurringJob.RemoveIfExists(jobId);
            RecurringJob.AddOrUpdate<TCareMessageJobService>(jobId, x => x.Run(tenant), $"* * * * *", TimeZoneInfo.Local);
            return NoContent();
        }
    }
}