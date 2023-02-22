using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMTDentalWebhook.Options;
using TMTDentalWebhook.Services;
using TMTDentalWebhook.Tools;
using Umbraco.Web.Models.Webhooks;

namespace TMTDentalWebhook.Controllers
{
    public class FacebookController : ControllerBase
    {
        private readonly TenantDbContext _tenantContext;
        private readonly AppSettings _appSettings;

        public FacebookController(TenantDbContext tenantContext, IOptions<AppSettings> appSettings)
        {
            _tenantContext = tenantContext;
            _appSettings = appSettings?.Value;
        }

        [FacebookWebHook]
        public async Task<IActionResult> Facebook(string id, JObject data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var obj = data["object"].Value<string>();
            if (obj == "page")
            {
                var pageId = data.SelectToken("$.entry[0].id").Value<string>();
                var tenantPages = await _tenantContext.TenantFacebookPages.Where(x => x.PageId == pageId).Include(x => x.Tenant).ToListAsync();
                foreach (var tenantPage in tenantPages)
                {
                    var httpClient = new HttpClient();
                    await httpClient.PostAsJsonAsync($"{_appSettings.Schema}://{tenantPage.Tenant.Hostname}.{_appSettings.CatalogDomain}/api/FacebookWebHook", data);
                }
            }

            return Ok();
        }

       
    }
}