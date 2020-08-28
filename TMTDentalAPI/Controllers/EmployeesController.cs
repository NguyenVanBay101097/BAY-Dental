using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : BaseApiController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IHrPayrollStructureTypeService _structureTypeService;
        private readonly IMapper _mapper;

        public EmployeesController(IEmployeeService employeeService, IHrPayrollStructureTypeService structureTypeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _structureTypeService = structureTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] EmployeePaged val)
        {
            var result = await _employeeService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _employeeService.SearchQuery(x => x.Id == id).Include(x => x.Category).Include(x => x.StructureType).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<EmployeeDisplay>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var employee = _mapper.Map<Employee>(val);
            employee.CompanyId = CompanyId;


            await _employeeService.CreateAsync(employee);

            val.Id = employee.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, EmployeeDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var employee = await _employeeService.SearchQuery(x => x.Id == id).Include(x => x.Category)
                .Include(x => x.ChamCongs)
                .Include(x => x.Company)
                .Include(x => x.StructureType)
                .Include("StructureType.DefaultResourceCalendar")
                .Include("StructureType.DefaultStruct")
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            employee = _mapper.Map(val, employee);

            await _employeeService.UpdateAsync(employee);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();
            await _employeeService.DeleteAsync(employee);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(EmployeePaged val)
        {
            var result = await _employeeService.GetAutocompleteAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchRead(EmployeePaged val)
        {
            var result = await _employeeService.GetPagedResultAsync(val);
            return Ok(result);
        }
    }
}