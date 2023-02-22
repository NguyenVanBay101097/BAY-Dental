using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalWebhook.Tools;

namespace TMTDentalWebhook.Services
{
    public interface IWebhookService
    {
        Task PushRequestToRelatedTenants(string pageId, UpdateObject data);
    }
}
