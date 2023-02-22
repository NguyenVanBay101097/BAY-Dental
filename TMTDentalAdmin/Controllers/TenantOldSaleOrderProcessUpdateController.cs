using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantOldSaleOrderProcessUpdateController : ControllerBase
    {
        private readonly ITenantOldSaleOrderProcessUpdateService _tenantOldSaleOrderProcessUpdateService;
        private readonly ITenantService _tenantService;
        private readonly AdminAppSettings _appSettings;

        public TenantOldSaleOrderProcessUpdateController(ITenantOldSaleOrderProcessUpdateService tenantOldSaleOrderProcessUpdateService,
            ITenantService tenantService,
            IOptions<AdminAppSettings> appSettings)
        {
            _tenantOldSaleOrderProcessUpdateService = tenantOldSaleOrderProcessUpdateService;
            _tenantService = tenantService;
            _appSettings = appSettings?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TenantOldSaleOrderProcessUpdateFilterPagedVM val)
        {
            var result = await _tenantOldSaleOrderProcessUpdateService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LoadAll()
        {
            if (_tenantOldSaleOrderProcessUpdateService.SearchQuery().Any())
                throw new Exception("Đã load dữ liệu update");

            var tenants = await _tenantService.SearchQuery().ToListAsync();
            var updates = new List<TenantOldSaleOrderProcessUpdate>();
            foreach (var tenant in tenants)
                updates.Add(new TenantOldSaleOrderProcessUpdate { TenantId = tenant.Id });

            await _tenantOldSaleOrderProcessUpdateService.CreateAsync(updates);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ProcessAll()
        {
            var processList = await _tenantOldSaleOrderProcessUpdateService.SearchQuery(x => x.State == "draft" || x.State == "exception").Include(x => x.Tenant).ToListAsync();
            var processDict = processList.ToDictionary(x => x.Id, x => x);
            var tasks = processList.Select(x => PrepareTaskProcessUpdate(x));
            var limit = 10;
            var offset = 0;
            var subTasks = tasks.Skip(offset).Take(limit).ToList();
            while(subTasks.Any())
            {
                var results = await Task.WhenAll(subTasks);
                foreach(var result in results)
                {
                    if (result != null)
                    {
                        var process = processDict[result.Id];
                        process.State = result.State;
                    }
                }

                offset += limit;
                subTasks = tasks.Skip(offset).Take(limit).ToList();
            }

            await _tenantOldSaleOrderProcessUpdateService.UpdateAsync(processDict.Values);

            return NoContent();
        }

        private async Task<ProcessUpdateResult> PrepareTaskProcessUpdate(TenantOldSaleOrderProcessUpdate item)
        {
            using (var client = new HttpClient())
            {
                var tenant = item.Tenant;
                try
                {
                    var response = await client.GetAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/Web/OldSaleOrderPaymentProcessUpdate");

                    var statusCode = (int)response.StatusCode;
                    if (response.IsSuccessStatusCode || statusCode == 410)
                        return new ProcessUpdateResult { Id = item.Id, State = "done" };
                    return new ProcessUpdateResult { Id = item.Id, State = "exception" };
                }
                catch
                {
                    return new ProcessUpdateResult { Id = item.Id, State = "exception" };
                }
            }
        }

        private class ProcessUpdateResult
        {
            public Guid Id { get; set; }

            public string State { get; set; }
        }
    }
}
