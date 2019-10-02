using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : BaseApiController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(IAppointmentService appointmentService,
            IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]AppointmentPaged appointmentPaged)
        {
            var result = await _appointmentService.GetPagedResultAsync(appointmentPaged);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _appointmentService.GetAppointmentDisplayAsync(id);    
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<Appointment>(val);
            await _appointmentService.CreateAsync(category);

            return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, AppointmentDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var category = await _appointmentService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _appointmentService.UpdateAsync(category);

            return NoContent();
        }

        [HttpPatch("PatchMulti")]
        public async Task<IActionResult> PatchMulti()
        {
            var dateFrom = DateTime.Today.AddDays(-3);
            var query = await _appointmentService.SearchQuery(x => x.Date >= dateFrom && x.Date < DateTime.Now && x.State.ToLower() == "confirmed").ToListAsync();
            var list = _mapper.Map<IEnumerable<AppointmentPatch>>(query);
            foreach (var item in list)
            {
                var entity = await _appointmentService.GetByIdAsync(item.Id);
                if (entity == null)
                {
                    return NotFound();
                }               
                var patch = new JsonPatchDocument<AppointmentPatch>();
                patch.Replace(x => x.State, "expired");
                var entityMap = _mapper.Map<AppointmentPatch>(entity);
                patch.ApplyTo(entityMap);

                entity = _mapper.Map(entityMap, entity);
                await _appointmentService.UpdateAsync(entity);
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument<AppointmentPatch> apnPatch)
        {
            var entity = await _appointmentService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var entityMap = _mapper.Map<AppointmentPatch>(entity);
            apnPatch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);
            await _appointmentService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _appointmentService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _appointmentService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(AppointmentDefaultGet val)
        {
            var res = await _appointmentService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("CountAppointment")]
        public async Task<IActionResult> CountAppointment(DateFromTo dateFromTo)
        {
            var res = await _appointmentService.CountAppointment(dateFromTo);
            return Ok(res);
        }

        //[HttpPut("CountAppointment")]
        //public async Task<IActionResult> HandleExpiredAppointments()
        //{
        //    var res = await _appointmentService.CountAppointment(dateFromTo);
        //    return Ok(res);
        //}
    }
}