using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Hangfire;
using Infrastructure.Services;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.Webhooks;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloWebHookController : ControllerBase
    {
        private readonly ITCareMessagingTraceService _tCareMessagingTraceService;
        private readonly TenantDbContext _tenantContext;
        private readonly AppSettings _appSettings;
        private readonly IFacebookWebhookJobService _webhookJobService;
        private readonly AppTenant _tenant;

        public ZaloWebHookController(ITCareMessagingTraceService tCareMessagingTraceService, TenantDbContext tenantContext, IOptions<AppSettings> appSettings,
            IFacebookWebhookJobService webhookJobService, ITenant<AppTenant> tenant)
        {
            _tCareMessagingTraceService = tCareMessagingTraceService;
            _tenantContext = tenantContext;
            _appSettings = appSettings?.Value;
            _webhookJobService = webhookJobService;
            _tenant = tenant?.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject data)
        {

            var whzl = data.ToObject<ZaloWebHook>();
            if (whzl.EventName == "user_seen_message")
            {
                foreach (var mgsId in whzl.Message.mgsIds)
                {
                    var watermark = whzl.Timestamp.ToLocalTime();
                    var db = _tenant != null ? _tenant.Hostname : "localhost";
                    BackgroundJob.Enqueue(() => _webhookJobService.ProcessRead(db, watermark, whzl.Recipient.Id, whzl.Sender.Id));

                    //var traces = await _tCareMessagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Opened.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.PSID == whzl.Recipient.Id && x.Type == "zalo" && x.ChannelSocial.PageId == whzl.Sender.Id && x.MessageId == mgsId).ToListAsync();
                    ////var traces = await _messagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Opened.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.UserProfile.PSID == messaging.Sender.Id).ToListAsync();
                    //foreach (var trace in traces)
                    //    trace.Opened = watermark;

                    //await _tCareMessagingTraceService.UpdateAsync(traces);
                    //await _tCareMessagingTraceService.AddTagWebhook(traces, "read");
                }

            }
            else if (whzl.EventName == "user_received_message")
            {
                var watermark = whzl.Timestamp.ToLocalTime();
                var db = _tenant != null ? _tenant.Hostname : "localhost";
                BackgroundJob.Enqueue(() => _webhookJobService.ProcessDelivery(db, watermark, whzl.Recipient.Id, whzl.Sender.Id));

                //var traces = await _tCareMessagingTraceService.SearchQuery(x => !string.IsNullOrEmpty(x.MessageId) && !x.Delivery.HasValue && x.Sent.HasValue && x.Sent <= watermark && x.PSID == whzl.Recipient.Id && x.Type == "zalo" && x.ChannelSocial.PageId == whzl.Sender.Id && x.MessageId == whzl.Message.Mgsid).ToListAsync();
                //foreach (var trace in traces)
                //    trace.Delivery = watermark;

                //await _tCareMessagingTraceService.UpdateAsync(traces);
                //await _tCareMessagingTraceService.AddTagWebhook(traces, "unread");
            }
            return Ok();
        }

    }

}