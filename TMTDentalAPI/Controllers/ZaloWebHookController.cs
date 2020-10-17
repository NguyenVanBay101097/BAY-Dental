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
        private readonly TenantDbContext _tenantContext;
        private readonly AppSettings _appSettings;
        private readonly IFacebookWebhookJobService _webhookJobService;
        private readonly AppTenant _tenant;

        public ZaloWebHookController(TenantDbContext tenantContext, IOptions<AppSettings> appSettings,
            IFacebookWebhookJobService webhookJobService, ITenant<AppTenant> tenant)
        {
            _tenantContext = tenantContext;
            _appSettings = appSettings?.Value;
            _webhookJobService = webhookJobService;
            _tenant = tenant?.Value;
        }

        [HttpPost]
        public IActionResult Post([FromBody]JObject data)
        {
            var db = _tenant != null ? _tenant.Hostname : "localhost";
            BackgroundJob.Enqueue<FacebookWebhookJobService>(x => x.ProcessZalo(db, data));

            //var whzl = data.ToObject<ZaloWebHook>();
            //if (whzl.EventName == "user_seen_message")
            //{
            //    foreach (var mgsId in whzl.Message.mgsIds)
            //    {
            //        var watermark = whzl.Timestamp.ToLocalTime();
            //        var db = _tenant != null ? _tenant.Hostname : "localhost";
            //        BackgroundJob.Enqueue(() => _webhookJobService.ProcessRead(db, watermark, whzl.Recipient.Id, whzl.Sender.Id));
            //    }

            //}
            //else if (whzl.EventName == "user_received_message")
            //{
            //    var watermark = whzl.Timestamp.ToLocalTime();
            //    var db = _tenant != null ? _tenant.Hostname : "localhost";
            //    BackgroundJob.Enqueue(() => _webhookJobService.ProcessDelivery(db, watermark, whzl.Recipient.Id, whzl.Sender.Id));
            //}
            //else if (whzl.EventName == "user_send_text")
            //{
            //    var db = _tenant != null ? _tenant.Hostname : "localhost";
            //    BackgroundJob.Enqueue(() => _webhookJobService.ProcessAddUserProfile(db, whzl.Sender.Id, whzl.Recipient.Id));
            //}

            return Ok();
        }

    }

}