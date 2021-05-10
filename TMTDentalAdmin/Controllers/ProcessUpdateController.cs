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

namespace TMTDentalAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessUpdateController : BaseApiController
    {
        private readonly ITenantService _tenantService;
        private readonly AdminAppSettings _appSettings;

        public ProcessUpdateController(ITenantService tenantService,
            IOptions<AdminAppSettings> appSettings)
        {
            _tenantService = tenantService;
            _appSettings = appSettings?.Value;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateOldSaleOrder(IEnumerable<Guid> ids)
        {
            var tenants = await _tenantService.SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach(var tenant in tenants)
            {
                if (tenant.IsProcessUpdateOldSaleOrder)
                    continue;

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/companies/setuptenant");
                    if (response.IsSuccessStatusCode)
                    {
                        tenant.IsProcessUpdateOldSaleOrder = true;
                        await _tenantService.UpdateAsync(tenant);
                    }
                }
            }

            return NoContent();
        }
    }
}
