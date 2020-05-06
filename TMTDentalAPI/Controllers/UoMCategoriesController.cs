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
        public async Task<IActionResult> Create(UoMCategorySave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
           var type = _mapper.Map<UoMCategory>(val);
            var res = await _uoMCategoryService.CreateAsync(type);
            return Ok(res);
        }

        [HttpPut("{id}")]
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
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _uoMCategoryService.GetByIdAsync(id);
            if (type == null)
                return NotFound();
            await _uoMCategoryService.DeleteAsync(type);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _uoMCategoryService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (res == null)
                return NotFound();
            return Ok(res);
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] UoMCategoryPaged val)
        {
            var res = await _uoMCategoryService.GetPagedResultAsync(val);
            return Ok(res);
        }

    }
}