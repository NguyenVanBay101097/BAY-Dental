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

        public FacebookWebHookController(IMarketingTraceService marketingTraceService)
        {
            _marketingTraceService = marketingTraceService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject data)
        {
            var wh = data.ToObject<FacebookWebHook>();
            foreach(var entry in wh.Entry)
            {
                if (entry.Messaging != null && entry.Messaging.Length > 0)
                {
                    var messaging = entry.Messaging[0];
                    if (messaging.Read != null)
                    {
                        var watermark = messaging.Read.Watermark;
                        var traces = await _marketingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Read.HasValue && x.Sent.HasValue && x.Sent <= watermark).ToListAsync();
                        foreach (var trace in traces)
                            trace.Read = watermark;
                        await _marketingTraceService.UpdateAsync(traces);
                    }
                    else if (messaging.Delivery != null)
                    {
                        var watermark = messaging.Delivery.Watermark;
                        var traces = await _marketingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Delivery.HasValue && x.Sent.HasValue && x.Sent <= watermark).ToListAsync();
                        foreach (var trace in traces)
                            trace.Delivery = watermark;
                        await _marketingTraceService.UpdateAsync(traces);
                    }
                }
            }
            return Ok();
        }
    }
}