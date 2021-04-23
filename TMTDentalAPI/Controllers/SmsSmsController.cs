using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsSmsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISmsSmsService _smsSmsService;
        public SmsSmsController(IMapper mapper, ISmsSmsService smsSmsService)
        {
            _mapper = mapper;
            _smsSmsService = smsSmsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedResult([FromQuery] SmsSmsPaged val)
        {
            var res = await _smsSmsService.GetPaged(val);
            return Ok(res);
        }

    }
}
