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
    public class FacebookOAuthController : ControllerBase
    {
        private readonly IFacebookOAuthService _facebookOAuthService;
        public FacebookOAuthController(IFacebookOAuthService facebookOAuthService)
        {
            _facebookOAuthService = facebookOAuthService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExchangeToken(string access_token)
        {
            var result = await _facebookOAuthService.ExchangeLiveLongUserAccessToken(access_token);
            return Ok(result);
        }
    }
}