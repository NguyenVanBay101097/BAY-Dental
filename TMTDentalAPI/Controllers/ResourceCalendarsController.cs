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
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Mapping;
using Umbraco.Web.Models;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceCalendarsController : BaseApiController
    {
        private readonly IResourceCalendarService _resourceCalendarService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IResourceCalendarAttendanceService _resourceCalendarAttendanceService;
        public ResourceCalendarsController(IResourceCalendarAttendanceService resourceCalendarAttendanceService, IResourceCalendarService resourceCalendarService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _resourceCalendarService = resourceCalendarService;
            _resourceCalendarAttendanceService = resourceCalendarAttendanceService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ResourceCalendarPaged val)
        {
            if (val == null && !ModelState.IsValid)
                return BadRequest();

            var res = await _resourceCalendarService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _resourceCalendarService.GetDisplayAsync(id);
            if (model == null)
                return BadRequest();

            var res = _mapper.Map<ResourceCalendarDisplay>(model);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResourceCalendarSave val)
        {
            if (val == null)
                return BadRequest();
            var model = _mapper.Map<ResourceCalendar>(val);
            await _unitOfWork.BeginTransactionAsync();
            model = await _resourceCalendarService.CreateAsync(model);
            _unitOfWork.Commit();
            var res = _mapper.Map<ResourceCalendarBasic>(model);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceCalendarSave val)
        {
            var model = await _resourceCalendarService.SearchQuery(x => x.Id == id).Include(x => x.ResourceCalendarAttendances).FirstOrDefaultAsync();
            if (val == null || model == null)
                return BadRequest();
            if (model.ResourceCalendarAttendances != null)
            {
                await _resourceCalendarAttendanceService.DeleteAsync(model.ResourceCalendarAttendances.AsEnumerable());
            }

            model = _mapper.Map(val, model);
            await _unitOfWork.BeginTransactionAsync();
            await _resourceCalendarService.UpdateAsync(model);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resourceCalendar = await _resourceCalendarService.GetByIdAsync(id);
            if (resourceCalendar == null)
                return BadRequest();
            await _resourceCalendarService.DeleteAsync(resourceCalendar);
            return NoContent();
        }


    }
}
