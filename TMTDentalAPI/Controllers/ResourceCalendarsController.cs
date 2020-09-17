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
using TMTDentalAPI.JobFilters;
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
        [CheckAccess(Actions = "Salary.ResourceCalendar.Read")]
        public async Task<IActionResult> Get([FromQuery] ResourceCalendarPaged val)
        {
            if (val == null && !ModelState.IsValid)
                return BadRequest();

            var res = await _resourceCalendarService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Salary.ResourceCalendar.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _resourceCalendarService.GetDisplayAsync(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Salary.ResourceCalendar.Create")]
        public async Task<IActionResult> Create(ResourceCalendarSave val)
        {
            if (val == null)
                return BadRequest();
 
            await _unitOfWork.BeginTransactionAsync();
            var res = await _resourceCalendarService.CreateResourceCalendar(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<ResourceCalendarBasic>(res);

            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Salary.ResourceCalendar.Update")]
        public async Task<IActionResult> Update(Guid id, ResourceCalendarSave val)
        {
            var model = await _resourceCalendarService.SearchQuery(x => x.Id == id).Include(x => x.Attendances).FirstOrDefaultAsync();
            if (val == null || model == null)
                return BadRequest();
          
            await _unitOfWork.BeginTransactionAsync();
            await _resourceCalendarService.UpdateResourceCalendar(id,val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Salary.ResourceCalendar.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resourceCalendar = await _resourceCalendarService.SearchQuery(x => x.Id == id).Include(x => x.Attendances).Include(x=>x.Leaves).ToListAsync();
            if (resourceCalendar == null)
                return BadRequest();

            await _resourceCalendarService.DeleteAsync(resourceCalendar);

            return NoContent();
        }

        [HttpGet("[action]")]
        public IActionResult DefaultGet()
        {
            var res = _resourceCalendarService.DefaultGet();
            return Ok(res);
        }
    }
}
