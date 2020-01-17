using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloAuthConfigController : BaseApiController
    {
        private readonly ZaloAuthConfig _zaloAuthConfig;
        public ZaloAuthConfigController(IOptions<ZaloAuthConfig> zaloAuthConfig)
        {
            _zaloAuthConfig = zaloAuthConfig?.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_zaloAuthConfig);
        }
    }
}