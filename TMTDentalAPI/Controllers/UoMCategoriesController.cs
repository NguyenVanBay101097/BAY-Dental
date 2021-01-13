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
    public class UoMCategoriesController : ControllerBase
    {
        private readonly IUoMCategoryService _uoMCategoryService;
        private readonly IMapper _mapper;
        public UoMCategoriesController(IMapper mapper, IUoMCategoryService uoMCategoryService)
        {
            _uoMCategoryService = uoMCategoryService;
            _mapper = mapper;
        }


        [HttpPost]
        [CheckAccess(Actions = "UoM.UoMCategory.Create")]
        public async Task<IActionResult> Create(UoMCategorySave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
           var type = _mapper.Map<UoMCategory>(val);
            var res = await _uoMCategoryService.CreateAsync(type);
            return Ok(res);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "UoM.UoMCategory.Update")]
        public async Task<IActionResult> Update(Guid id, UoMCategorySave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var type = await _uoMCategoryService.GetByIdAsync(id);
            if (type == null)
                return NotFound();
            type = _mapper.Map(val, type);
            await _uoMCategoryService.UpdateAsync(type);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "UoM.UoMCategory.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _uoMCategoryService.GetByIdAsync(id);
            if (type == null)
                return NotFound();
            await _uoMCategoryService.DeleteAsync(type);
            return NoContent();
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "UoM.UoMCategory.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _uoMCategoryService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (res == null)
                return NotFound();
            return Ok(res);
        }


        [HttpGet]
        [CheckAccess(Actions = "UoM.UoMCategory.Read")]
        public async Task<IActionResult> Get([FromQuery] UoMCategoryPaged val)
        {
            var res = await _uoMCategoryService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpPost("Autocomplete")]
        [CheckAccess(Actions = "UoM.UoMCategory.Read")]
        public async Task<IActionResult> Autocomplete(UoMCategoryPaged val)
        {
            var res = await _uoMCategoryService.GetAutocompleteAsync(val);
            return Ok(res);
        }

    }
}