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
    public class ResourceCalendarController : BaseApiController
    {

        private readonly IResourceCalendarService _ResourceCalendarService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public ResourceCalendarController(IResourceCalendarService ResourceCalendarService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _ResourceCalendarService = ResourceCalendarService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await _ResourceCalendarService.GetAll();
            return Ok(_mapper.Map<IEnumerable<ResourceCalendarDisplay>>(res));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var ResourceCalendar = await _ResourceCalendarService.GetResourceCalendarDisplay(id);
            if (ResourceCalendar == null)
                return NotFound();
            var res = _mapper.Map<ResourceCalendarDisplay>(ResourceCalendar);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResourceCalendarSave val)
        {
            var entitys = _mapper.Map<ResourceCalendar>(val);

            await _unitOfWork.BeginTransactionAsync();
            await _ResourceCalendarService.CreateAsync(entitys);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<ResourceCalendarDisplay>(entitys));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResourceCalendarSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _ResourceCalendarService.GetResourceCalendarDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);

            await _ResourceCalendarService.UpdateAsync(str);

            return NoContent();
        }
      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var ResourceCalendar = await _ResourceCalendarService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (ResourceCalendar == null)
            {
                return NotFound();
            }
            await _ResourceCalendarService.DeleteAsync(ResourceCalendar);
            return NoContent();
        }
    }
}
