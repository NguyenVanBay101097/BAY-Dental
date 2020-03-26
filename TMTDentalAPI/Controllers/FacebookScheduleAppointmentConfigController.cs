using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookScheduleAppointmentConfigController : BaseApiController
    {
        private readonly IFacebookScheduleAppointmentConfigService _facebookScheduleAppointmentConfigService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public FacebookScheduleAppointmentConfigController(IFacebookScheduleAppointmentConfigService facebookScheduleAppointmentConfigService,
            IMapper mapper , IUnitOfWorkAsync unitOfWork)
        {
            _facebookScheduleAppointmentConfigService = facebookScheduleAppointmentConfigService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        //[HttpGet]
        //public async Task<IActionResult> Get([FromQuery] FacebookScheduleAppointmentConfigPaged val)
        //{
        //    var res = await _facebookScheduleAppointmentConfigService.GetPagedResultAsync(val);
        //    return Ok(res);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var facebookSchedule = await _facebookScheduleAppointmentConfigService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            return Ok(facebookSchedule);
        }
        [HttpPost]
        public async Task<IActionResult> Create(FacebookScheduleAppointmentConfigSave val)
        {
            var result = await _facebookScheduleAppointmentConfigService.CreateFBSheduleConfig(val);

            var basic = _mapper.Map<FacebookScheduleAppointmentConfigBasic>(result);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id , FacebookScheduleAppointmentConfigSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _facebookScheduleAppointmentConfigService.UpdateFBSheduleConfig(id, val);
            _unitOfWork.Commit();
            var basic = _mapper.Map<FacebookScheduleAppointmentConfigBasic>(result);
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