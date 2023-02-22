using Infrastructure.TenantData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TMTDentalWebhook.Tools;

namespace TMTDentalWebhook.Services
{
    public class WebhookService: IWebhookService
    {
        private readonly TenantDbContext _tenantContext;

        public WebhookService(TenantDbContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public async Task PushRequestToRelatedTenants(string pageId, UpdateObject data)
        {
            var tenants = _tenantContext.TenantFacebookPages.Where(x => x.PageId == pageId).ToList();
            foreach(var tenant in tenants)
            {
                var client = new HttpClient();
                await client.PostAsJsonAsync("tenant1/api/Webhooks", data);
            }
        }
    }
}
