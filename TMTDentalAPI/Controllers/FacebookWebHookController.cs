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
        private readonly ITCareMessagingTraceService _tCareMessagingTraceService;

        public FacebookWebHookController(IMarketingTraceService marketingTraceService,
            IFacebookMessagingTraceService messagingTraceService, ITCareMessagingTraceService tCareMessagingTraceService)
        {
            _marketingTraceService = marketingTraceService;
            _messagingTraceService = messagingTraceService;
            _tCareMessagingTraceService = tCareMessagingTraceService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject data)
        {
            var wh = data.ToObject<FacebookWebHook>();
            foreach (var entry in wh.Entry)
            {
                if (entry.Messaging != null)
                {
                    foreach (var messaging in entry.Messaging)
                    {

                        if (messaging.Read != null)
                        {
                            var watermark = messaging.Read.Watermark.ToLocalTime();
                            var traces = await _tCareMessagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Read.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.PSID == messaging.Sender.Id && x.Type == "facebook").ToListAsync();
                            //var traces = await _messagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Opened.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.UserProfile.PSID == messaging.Sender.Id).ToListAsync();  
                            foreach(var trace in traces)                            
                                trace.Read = watermark;
                                                       
                            await _tCareMessagingTraceService.UpdateAsync(traces);
                            await _tCareMessagingTraceService.AddTagWebhook(traces, "read");        

                        }

                        if (messaging.Delivery != null)
                        {
                            var watermark = messaging.Delivery.Watermark.ToLocalTime();
                            var traces = await _tCareMessagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Delivery.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.PSID == messaging.Sender.Id && x.Type == "facebook").ToListAsync();                           
                           

                            foreach (var trace in traces)
                             trace.Delivery = watermark;

                            await _tCareMessagingTraceService.UpdateAsync(traces);
                            await _tCareMessagingTraceService.AddTagWebhook(traces, "unread");



                        }
                    }
                }
            }
            return Ok();
        }
    }
}