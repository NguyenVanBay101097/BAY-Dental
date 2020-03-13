using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookConnectPagesController : ControllerBase
    {
        private readonly IFacebookConnectPageService _facebookConnectPageService;

        public FacebookConnectPagesController(IFacebookConnectPageService facebookConnectPageService)
        {
            _facebookConnectPageService = facebookConnectPageService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddConnect(IEnumerable<Guid> ids)
        {
            await _facebookConnectPageService.AddConnect(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveConnect(IEnumerable<Guid> ids)
        {
            await _facebookConnectPageService.RemoveConnect(ids);
            return NoContent();
        }
    }
}