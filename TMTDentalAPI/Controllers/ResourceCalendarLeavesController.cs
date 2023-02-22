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
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceCalendarLeavesController : BaseApiController
    {
        private readonly IResourceCalendarLeaveService _resourceCalendarLeaveService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ResourceCalendarLeavesController(IResourceCalendarLeaveService resourceCalendarLeaveService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _resourceCalendarLeaveService = resourceCalendarLeaveService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get(ResourceCalendarLeavePaged val)
        {
            if (val == null && !ModelState.IsValid)
                return BadRequest();
            var res = await _resourceCalendarLeaveService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _resourceCalendarLeaveService.GetByIdAsync(id);
            if (res == null)
                return NotFound();
            return Ok(_mapper.Map<ResourceCalendarAttendanceDisplay>(res));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResourceCalendarAttendanceSave val)
        {
            if (val == null) return BadRequest();
            var entity = _mapper.Map<ResourceCalendarLeaves>(val);
            await _unitOfWork.BeginTransactionAsync();
            var res = await _resourceCalendarLeaveService.CreateAsync(entity);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<ResourceCalendarAttendanceDisplay>(res));
        }
        
        [HttpPut("{ud}")]
        public async Task<IActionResult> Update(Guid id, ResourceCalendarAttendanceSave val)
        {
            var model = await _resourceCalendarLeaveService.GetByIdAsync(id);
            if (model == null) return NotFound();
            _mapper.Map(val,model);
            await _unitOfWork.BeginTransactionAsync();
            await _resourceCalendarLeaveService.UpdateAsync(model);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _resourceCalendarLeaveService.GetByIdAsync(id);
            if (model == null) return NoContent();
            await _resourceCalendarLeaveService.DeleteAsync(model);
            return NoContent();
        }
    }
}
