using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookScheduleAppointmentConfigController : BaseApiController
    {
        private readonly IFacebookScheduleAppointmentConfigService _facebookScheduleAppointmentConfigService;
        private readonly IMapper _mapper;

        public FacebookScheduleAppointmentConfigController(IFacebookScheduleAppointmentConfigService facebookScheduleAppointmentConfigService,
            IMapper mapper)
        {
            _facebookScheduleAppointmentConfigService = facebookScheduleAppointmentConfigService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacebookScheduleAppointmentConfigSave val)
        {
            var campaign = await _facebookScheduleAppointmentConfigService.CreateFBSheduleConfig(val);

            var basic = _mapper.Map<FacebookScheduleAppointmentConfigBasic>(campaign);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionStartShedule(IEnumerable<Guid> ids)
        {
            await _facebookScheduleAppointmentConfigService.ActionStart(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionStopShedule(IEnumerable<Guid> ids)
        {
            await _facebookScheduleAppointmentConfigService.ActionStop(ids);
            return NoContent();
        }
    }
}