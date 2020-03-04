using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookConfigPagesController : ControllerBase
    {
        private readonly IFacebookConfigPageService _configPageService;
        private readonly IFacebookConversationService _conversationService;

        public FacebookConfigPagesController(IFacebookConfigPageService configPageService,
            IFacebookConversationService conversationService)
        {
            _configPageService = configPageService;
            _conversationService = conversationService;
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> SyncData(Guid id)
        {
            await _configPageService.SyncData(id);
            return NoContent();
        }

        [HttpGet("{pageId}/[action]")]
        public async Task<IActionResult> Conversations(string pageId, Guid configId)
        {
            var configPage = await _configPageService.SearchQuery(x => x.PageId == pageId && x.ConfigId == configId).FirstOrDefaultAsync();
            if (configPage == null)
                return NotFound();
            var access_token = configPage.PageAccessToken;
            var res = await _conversationService.SearchQuery(x => x.FacebookPageId == pageId).Select(x => new
            {
                Snippet = x.Snippet,
                UserId = x.UserId,
                User = new
                {
                    FacebookProfilePicture = "https://graph.facebook.com/" + x.User.PsId + "/picture?access_token=" + access_token,
                    FacebookUserId = x.User.PsId,
                    FacebookUserName = x.User.Name
                }
            }).ToListAsync();
            return Ok(res);
        }
    }
}