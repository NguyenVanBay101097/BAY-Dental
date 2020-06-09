using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
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
    public class ZaloWebHookController : ControllerBase
    {
        private readonly ITCareMessagingTraceService _tCareMessagingTraceService;
        public ZaloWebHookController(ITCareMessagingTraceService tCareMessagingTraceService)
        {
            _tCareMessagingTraceService = tCareMessagingTraceService;
        }

       

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject data)
        {
           
            var whzl = data.ToObject<ZaloWebHook>();

            if (whzl.EventName == "user_received_message")
            {
                foreach (var messaging in whzl.Message.ids)
                {
               
                        var watermark = whzl.Timestamp.ToLocalTime();
                        var traces = await _tCareMessagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Read.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.PSID == whzl.Sender.Id && x.Type == "zalo").ToListAsync();
                        //var traces = await _messagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Opened.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.UserProfile.PSID == messaging.Sender.Id).ToListAsync();
                        foreach (var trace in traces)
                            trace.Read = watermark;

                        await _tCareMessagingTraceService.UpdateAsync(traces);
                    

                  
                }

            }else if (whzl.EventName == "user_received_message")
            {

                foreach (var messaging in whzl.Message.ids)
                {
                    var watermark = whzl.Timestamp.ToLocalTime();
                    var traces = await _tCareMessagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Delivery.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.PSID == whzl.Sender.Id && x.Type == "zalo").ToListAsync();
                    foreach (var trace in traces)
                        trace.Delivery = watermark;

                    await _tCareMessagingTraceService.UpdateAsync(traces);

                }
                
            }
            return Ok();
        }

    }

}