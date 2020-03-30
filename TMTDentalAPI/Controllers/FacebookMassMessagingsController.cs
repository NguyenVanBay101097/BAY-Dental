using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookMassMessagingsController : ControllerBase
    {
        private readonly IFacebookMassMessagingService _facebookMassMessagingService;
        private readonly IMapper _mapper;

        public FacebookMassMessagingsController(IFacebookMassMessagingService facebookMassMessagingService,
            IMapper mapper)
        {
            _facebookMassMessagingService = facebookMassMessagingService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FacebookMassMessagingPaged val)
        {
            var res = await _facebookMassMessagingService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacebookMassMessagingSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var messaging = _mapper.Map<FacebookMassMessaging>(val);
            await _facebookMassMessagingService.CreateAsync(messaging);

            var basic = _mapper.Map<FacebookMassMessagingBasic>(messaging);
            return Ok(basic);
        }
    }
}