using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailMessagesController : BaseApiController
    {
        private readonly IMailMessageService _mailMessageService;
        public MailMessagesController(IMailMessageService mailMessageService)
        {
            _mailMessageService = mailMessageService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MessageFetch(MailMessageFetch val)
        {
            var res = await _mailMessageService.MessageFetch(val);
            return Ok(res);
        }
    }
}