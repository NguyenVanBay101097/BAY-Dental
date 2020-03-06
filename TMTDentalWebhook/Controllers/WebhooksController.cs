using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TMTDentalWebhook.Options;
using TMTDentalWebhook.Services;
using TMTDentalWebhook.Tools;

namespace TMTDentalWebhook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        private readonly FacebookOptions _fbOptions;
        private readonly ILogger<WebhooksController> _log;

        public WebhooksController(IOptions<FacebookOptions> fbOptions,
            ILogger<WebhooksController> logger)
        {
            _fbOptions = fbOptions?.Value;
            _log = logger;
        }

        [HttpGet]
        public string Get([FromQuery(Name = "hub.mode")] string hub_mode,
            [FromQuery(Name = "hub.challenge")] string hub_challenge,
            [FromQuery(Name = "hub.verify_token")] string hub_verify_token)
        {
            if (_fbOptions.VerifyToken == hub_verify_token)
            {
                _log.LogInformation("Get received. Token OK : {0}", hub_verify_token);
                return hub_challenge;
            }
            else
            {
                _log.LogError("Error. Token did not match. Got : {0}, Expected : {1}", hub_verify_token, _fbOptions.VerifyToken);
                return "error. no match";
            }
        }

        [HttpPost]
        public IActionResult Post()
        {
            string json = "";
            try
            {
                using (StreamReader sr = new StreamReader(this.Request.Body))
                {
                    json = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _log.LogCritical("Error during body read.", ex);
            }

            if (_fbOptions.ShouldVerifySignature)
            {
                VerifySignature(json);
            }

            var updateObject = JsonConvert.DeserializeObject<UpdateObject>(json);
            if (updateObject.Object == "page")
            {
                var entry = updateObject.Entry[0];
                var pageId = entry.Id;

            }

            return Ok();
        }

        private void VerifySignature(string json)
        {
            var signatures = Request.Headers.Where(h => h.Key == "X-Hub-Signature").ToArray();
            if (signatures.Length == 0)
                throw new Exception("X-Hub-Signature not found");
            if (signatures.Length >= 2)
                throw new Exception("Many X-Hub-Signature found");
            string headerHash = signatures[0].Value;
            if (headerHash == null)
                throw new Exception("X-Hub-Signature is null");
            if (!headerHash.StartsWith("sha1="))
                throw new Exception("Unexpected format of X-Hub-Signature : " + headerHash);
            headerHash = headerHash.Substring(5);

            string myHash = Hash.ComputeHash(_fbOptions.AppSecret, json);
            if (myHash == null)
                throw new Exception("Unexpected null hash");
            if (!myHash.Equals(headerHash, StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Hash did not match. Expected {myHash}. But header was {headerHash}");
        }
    }
}