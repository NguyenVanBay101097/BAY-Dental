using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Hangfire;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.Webhooks;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookWebHookController : ControllerBase
    {
        private readonly IMarketingTraceService _marketingTraceService;
        private readonly IFacebookMessagingTraceService _messagingTraceService;
        private readonly IFacebookWebhookJobService _fbWebhookJobService;
        private readonly AppTenant _tenant;

        public FacebookWebHookController(IMarketingTraceService marketingTraceService,
            IFacebookMessagingTraceService messagingTraceService,
            IFacebookWebhookJobService fbWebhookJobService, ITenant<AppTenant> tenant)
        {
            _marketingTraceService = marketingTraceService;
            _messagingTraceService = messagingTraceService;
            _fbWebhookJobService = fbWebhookJobService;
            _tenant = tenant?.Value;
        }

        [HttpPost]
        public IActionResult Post([FromBody]JObject data)
        {
            var db = _tenant != null ? _tenant.Hostname : "localhost";
            BackgroundJob.Enqueue<FacebookWebhookJobService>(x => x.ProcessFacebook(db, data));
            return Ok();
        }
    }
}