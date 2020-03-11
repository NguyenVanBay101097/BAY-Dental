using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.Webhooks;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookMessageReadsWebhookController : ControllerBase
    {
        private readonly IMarketingTraceService _marketingTraceService;

        public FacebookMessageReadsWebhookController(IMarketingTraceService marketingTraceService)
        {
            _marketingTraceService = marketingTraceService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]FacebookMessengerReadsWebhook val)
        {
            if (val.Entry != null)
            {
                var messaging = val.Entry[0].Messaging[0];
                var watermark = messaging.Read.Watermark;
                var traces = await _marketingTraceService.SearchQuery(x => string.IsNullOrEmpty(x.MessageId) && !x.Read.HasValue && x.Sent.HasValue && x.Sent <= watermark).ToListAsync();
                foreach(var trace in traces)
                    trace.Read = watermark;
                await _marketingTraceService.UpdateAsync(traces);
            }
           
            return Ok();
        }
    }
}