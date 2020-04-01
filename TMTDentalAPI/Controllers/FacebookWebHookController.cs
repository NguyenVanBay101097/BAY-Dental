using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Umbraco.Web.Models.Webhooks;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookWebHookController : ControllerBase
    {
        private readonly IMarketingTraceService _marketingTraceService;
        private readonly IFacebookMessagingTraceService _messagingTraceService;

        public FacebookWebHookController(IMarketingTraceService marketingTraceService,
            IFacebookMessagingTraceService messagingTraceService)
        {
            _marketingTraceService = marketingTraceService;
            _messagingTraceService = messagingTraceService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject data)
        {
            var wh = data.ToObject<FacebookWebHook>();
            foreach(var entry in wh.Entry)
            {
                if (entry.Messaging != null)
                {
                    foreach(var messaging in entry.Messaging)
                    {
                        if (messaging.Read != null)
                        {
                            var watermark = messaging.Read.Watermark.ToLocalTime();
                            var traces = await _messagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Opened.HasValue && x.Sent.HasValue && x.Sent <= watermark).ToListAsync();
                            foreach (var trace in traces)
                                trace.Opened = watermark;

                            await _messagingTraceService.UpdateAsync(traces);
                        }
                        
                        if (messaging.Delivery != null)
                        {
                            var watermark = messaging.Delivery.Watermark.ToLocalTime();
                            var traces = await _messagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Delivered.HasValue && x.Sent.HasValue && x.Sent <= watermark).ToListAsync();
                            foreach (var trace in traces)
                                trace.Delivered = watermark;

                            await _messagingTraceService.UpdateAsync(traces);
                        }
                    }
                }
            }
            return Ok();
        }
    }
}