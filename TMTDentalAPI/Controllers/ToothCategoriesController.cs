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
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToothCategoriesController : BaseApiController
    {
        private readonly IToothCategoryService _toothCategoryService;
        private readonly IMapper _mapper;

        public ToothCategoriesController(IToothCategoryService toothCategoryService,
            IMapper mapper)
        {
            _toothCategoryService = toothCategoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int offset = 0,
          int limit = 20,
          string search = "")
        {
            var result = await _toothCategoryService.GetPagedResultAsync(offset: offset, limit: limit, search: search);

            var paged = new PagedResult2<ToothCategoryBasic>(result.TotalItems, offset, limit)
            {
                Items = _mapper.Map<IEnumerable<ToothCategoryBasic>>(result.Items),
            };

            return Ok(paged);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _toothCategoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ToothCategoryBasic>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ToothCategoryBasic val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<ToothCategory>(val);
            await _toothCategoryService.CreateAsync(category);

            return CreatedAtAction(nameof(Get), new { id = category.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ToothCategoryBasic val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var category = await _toothCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _toothCategoryService.UpdateAsync(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _toothCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _toothCategoryService.DeleteAsync(category);

            return NoContent();
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _toothCategoryService.GetAllBasic();
            return Ok(res);
        }

        [HttpGet("GetDefaultCategory")]
        public async Task<IActionResult> GetDefaultCategory()
        {
            var res = await _toothCategoryService.GetDefaultCategory();
            return Ok(res);
        }
    }
}