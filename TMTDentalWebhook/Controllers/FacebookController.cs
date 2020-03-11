using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMTDentalWebhook.Options;
using TMTDentalWebhook.Services;
using TMTDentalWebhook.Tools;
using Umbraco.Web.Models.Webhooks;

namespace TMTDentalWebhook.Controllers
{
    public class FacebookController : ControllerBase
    {
        [FacebookWebHook(Id = "It")]
        public IActionResult FacebookForIt(string id, JObject data)
        {
            return Ok();
        }

        [FacebookWebHook]
        public IActionResult Facebook(string id, string @event, JObject data)
        {
           return Ok();
        }
    }
}