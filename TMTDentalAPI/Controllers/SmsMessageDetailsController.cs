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
    public class SmsMessageDetailsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISmsMessageDetailService _smsMessageDetailService;
        public SmsMessageDetailsController(IMapper mapper, ISmsMessageDetailService smsMessageDetailService)
        {
            _mapper = mapper;
            _smsMessageDetailService = smsMessageDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedResult([FromQuery] SmsMessageDetailPaged val)
        {
            var res = await _smsMessageDetailService.GetPaged(val);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> RunJobSendSms()
        {
            await _smsMessageDetailService.RunJobSendSms();
            return Ok(); 
        }
    }
}
