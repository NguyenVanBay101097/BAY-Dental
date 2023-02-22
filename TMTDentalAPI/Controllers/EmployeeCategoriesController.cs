using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
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
    public class EmployeeCategoriesController : BaseApiController
    {
        private readonly IEmployeeCategoryService _employeeCategoryService;
        private readonly IMapper _mapper;

        public EmployeeCategoriesController(IEmployeeCategoryService employeeCategoryService,
            IMapper mapper)
        {
            _employeeCategoryService = employeeCategoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]EmployeeCategoryPaged val)
        {
            var result = await _employeeCategoryService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _employeeCategoryService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<EmployeeCategoryDisplay>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCategoryDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<EmployeeCategory>(val);
            await _employeeCategoryService.CreateAsync(category);

            val.Id = category.Id;
            return CreatedAtAction(nameof(Get), new { id = category.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, EmployeeCategoryDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var category = await _employeeCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _employeeCategoryService.UpdateAsync(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _employeeCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _employeeCategoryService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(EmployeeCategoryPaged val)
        {
            var result = await _employeeCategoryService.GetAutocompleteAsync(val);
            return Ok(result);
        }

        [HttpPost("Autocomplete2")]
        public async Task<IActionResult> Autocomplete2(EmployeeCategoryPaged val)
        {
            var result = await _employeeCategoryService.GetAutocompleteAsync(val);
            return Ok(result);
        }
    }
}