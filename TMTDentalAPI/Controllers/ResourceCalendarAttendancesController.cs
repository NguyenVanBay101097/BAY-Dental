using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceCalendarAttendancesController : BaseApiController
    {
        private readonly IResourceCalendarAttendanceService _resourceCalendarAttendanceService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public ResourceCalendarAttendancesController(IResourceCalendarAttendanceService resourceCalendarAttendanceService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _resourceCalendarAttendanceService = resourceCalendarAttendanceService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ResourceCalendarAttendancePaged val)
        {
            if (val == null && !ModelState.IsValid)
                return BadRequest();

            var res = await _resourceCalendarAttendanceService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetListResourceCalendadrAtt([FromQuery] ResourceCalendarAttendancePaged val)
        {
            if (val == null && !ModelState.IsValid)
                return BadRequest();
            var res = await _resourceCalendarAttendanceService.GetListResourceCalendadrAtt(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SetSequence(IList<ResourceCalendarAttendance> vals)
        {
            if (vals == null && !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _resourceCalendarAttendanceService.SetSequence(vals);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _resourceCalendarAttendanceService.GetByIdAsync(id);
            if (model == null)
                return BadRequest();

            var res = _mapper.Map<ResourceCalendarAttendanceDisplay>(model);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResourceCalendarAttendanceSave val)
        {
            if (val == null)
                return BadRequest();
            var model = _mapper.Map<ResourceCalendarAttendance>(val);
            await _unitOfWork.BeginTransactionAsync();
            model = await _resourceCalendarAttendanceService.CreateAsync(model);
            _unitOfWork.Commit();
            var res = _mapper.Map<ResourceCalendarAttendanceBasic>(model);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceCalendarAttendanceSave val)
        {
            var model = await _resourceCalendarAttendanceService.GetByIdAsync(id);
            if (val == null || model == null)
                return BadRequest();

            _mapper.Map(val, model);
            await _unitOfWork.BeginTransactionAsync();
            await _resourceCalendarAttendanceService.UpdateAsync(model);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resourceCalendar = await _resourceCalendarAttendanceService.GetByIdAsync(id);
            if (resourceCalendar == null)
                return BadRequest();
            await _resourceCalendarAttendanceService.DeleteAsync(resourceCalendar);
            return NoContent();
        }
    }
}
