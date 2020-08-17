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
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceCalendarAttendanceController : BaseApiController
    {

        private readonly IResourceCalendarAttendanceService _ResourceCalendarAttendanceService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public ResourceCalendarAttendanceController(IResourceCalendarAttendanceService ResourceCalendarAttendanceService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _ResourceCalendarAttendanceService = ResourceCalendarAttendanceService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await _ResourceCalendarAttendanceService.GetAll();
            return Ok(_mapper.Map<IEnumerable<ResourceCalendarAttendanceDisplay>>(res));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var ResourceCalendarAttendance = await _ResourceCalendarAttendanceService.GetResourceCalendarAttendanceDisplay(id);
            if (ResourceCalendarAttendance == null)
                return NotFound();
            var res = _mapper.Map<ResourceCalendarAttendanceDisplay>(ResourceCalendarAttendance);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResourceCalendarAttendanceSave val)
        {
            var entitys = _mapper.Map<ResourceCalendarAttendance>(val);

            await _unitOfWork.BeginTransactionAsync();
            await _ResourceCalendarAttendanceService.CreateAsync(entitys);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<ResourceCalendarAttendanceDisplay>(entitys));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceCalendarAttendanceSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _ResourceCalendarAttendanceService.GetResourceCalendarAttendanceDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);

            await _ResourceCalendarAttendanceService.UpdateAsync(str);

            return NoContent();
        }
      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var ResourceCalendarAttendance = await _ResourceCalendarAttendanceService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (ResourceCalendarAttendance == null)
            {
                return NotFound();
            }
            await _ResourceCalendarAttendanceService.DeleteAsync(ResourceCalendarAttendance);
            return NoContent();
        }
    }
}
