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
    public class IrConfigParametersController : ControllerBase
    {
        private readonly IIrConfigParameterService _irConfigParameterService;
        public IrConfigParametersController(IIrConfigParameterService irConfigParameterService)
        {
            _irConfigParameterService = irConfigParameterService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetParam([FromQuery]string key)
        {
            var value = await _irConfigParameterService.GetParam(key);
            return Ok(new { value });
        }
    }
}