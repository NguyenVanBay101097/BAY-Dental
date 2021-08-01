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
        private static HttpClient _client = new HttpClient();

        public ProcessUpdateController(ITenantService tenantService,
            IOptions<AdminAppSettings> appSettings)
        {
            _tenantService = tenantService;
            _appSettings = appSettings?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tenants = await _tenantService.SearchQuery(x => x.Version != "1.0.1.8").ToListAsync();
            foreach(var tenant in tenants)
            {
                var response = await _client.GetAsync($"https://{tenant.Hostname}.tdental.vn/ProcessUpdate");
            }

            return NoContent();
        }
    }
}
