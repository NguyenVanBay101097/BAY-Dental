using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAdminsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeAdminService _employeeAdminService;

        public EmployeeAdminsController(IMapper mapper, IEmployeeAdminService employeeAdminService)
        {
            _mapper = mapper;
            _employeeAdminService = employeeAdminService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] EmployeeAdminPaged val)
        {
            var result = await _employeeAdminService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _employeeAdminService.GetByIdAsync(id);
            var display = _mapper.Map<EmployeeAdminDisplay>(result);
            return Ok(display);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(EmployeeAdminSave val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();
            var res = _mapper.Map<EmployeeAdmin>(val);
            res = await _employeeAdminService.CreateAsync(res);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var model = await _employeeAdminService.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            await _employeeAdminService.DeleteAsync(model);
            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> Update(Guid id, EmployeeAdminUpdateViewModel val)
        {
            var employeeAdmin = await _employeeAdminService.GetByIdAsync(id);
            employeeAdmin = _mapper.Map(val, employeeAdmin);
            await _employeeAdminService.UpdateAsync(employeeAdmin);

            return NoContent();
        }
    }
}
