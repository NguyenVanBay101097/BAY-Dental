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
using TMTDentalAPI.JobFilters;
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
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> Get([FromQuery] EmployeePaged val)
        {
            var result = await _employeeService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _mapper.ProjectTo<EmployeeDisplay>(_employeeService.SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (res == null)
                return NotFound();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.Employee.Create")]
        public async Task<IActionResult> Create(EmployeeDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var employee = _mapper.Map<Employee>(val);
            employee.CompanyId = CompanyId;


            var partner = new Partner();
            employee.Partner = partner;
            UpdatePartnerToEmployee(employee);
            await _employeeService.CreateAsync(employee);

            val.Id = employee.Id;
            return Ok(val);
        }

        private void UpdatePartnerToEmployee (Employee employee)
        {
            if (employee.Partner == null) return;
            var pn = employee.Partner;
            pn.Name = employee.Name;
            pn.Employee = true;
            pn.Ref = employee.Ref;
            pn.Phone = employee.Phone;
            pn.Email = employee.Email;
            pn.BirthDay = employee.BirthDay.HasValue ? employee.BirthDay.Value.Day : 1;
            pn.BirthMonth = employee.BirthDay.HasValue ? employee.BirthDay.Value.Month : 1;
            pn.BirthYear = employee.BirthDay.HasValue ? employee.BirthDay.Value.Year : DateTime.Now.Year;
            pn.Barcode = employee.EnrollNumber;
            pn.Supplier = false;
            pn.Customer = false;
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Update")]
        public async Task<IActionResult> Update(Guid id, EmployeeDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var employee = await _employeeService.SearchQuery(x => x.Id == id).Include(x => x.Category)
                .Include(x => x.ChamCongs)
                .Include(x => x.Company)
                .Include(x => x.StructureType)
                .Include(x => x.Partner)
                .Include("StructureType.DefaultResourceCalendar")
                .Include("StructureType.DefaultStruct")
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            employee = _mapper.Map(val, employee);
            UpdatePartnerToEmployee(employee);
            await _employeeService.UpdateAsync(employee);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.Employee.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();
            await _employeeService.DeleteAsync(employee);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> Autocomplete(EmployeePaged val)
        {
            var result = await _employeeService.GetAutocompleteAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Employee.Read")]
        public async Task<IActionResult> SearchRead(EmployeePaged val)
        {
            var result = await _employeeService.GetPagedResultAsync(val);
            return Ok(result);
        }
    }
}