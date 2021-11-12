using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.Controllers;

namespace TMTDentalAPI.PublicApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessTokenController : BaseApiController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIrConfigParameterService _irConfigParameterService;
        public AccessTokenController(IHttpContextAccessor httpContextAccessor, IIrConfigParameterService irConfigParameterService)
        {
            _httpContextAccessor = httpContextAccessor;
            _irConfigParameterService = irConfigParameterService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _irConfigParameterService.GetParam("public.access_token");
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GenerateToken()
        {
            var access_token = Guid.NewGuid().ToString();
            var result = await _irConfigParameterService.SearchQuery(x => x.Key == "public.access_token").FirstOrDefaultAsync();          
            if(result == null)
            {
                await _irConfigParameterService.SetParam("public.access_token", access_token);
                return NoContent();
            };

            result.Value = access_token;
            await _irConfigParameterService.UpdateAsync(result);

            return NoContent();
        }

    }
}
