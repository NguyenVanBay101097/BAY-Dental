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
    public class TeethController : BaseApiController
    {
        private readonly IToothService _toothService;
        private readonly IMapper _mapper;

        public TeethController(IToothService toothService,
            IMapper mapper)
        {
            _toothService = toothService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int offset = 0,
          int limit = 20,
          string search = "")
        {
            var result = await _toothService.GetPagedResultAsync(offset: offset, limit: limit, search: search);

            var paged = new PagedResult2<ToothBasic>(result.TotalItems, offset, limit)
            {
                Items = _mapper.Map<IEnumerable<ToothBasic>>(result.Items),
            };

            return Ok(paged);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _toothService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ToothBasic>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ToothBasic val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<Tooth>(val);
            await _toothService.CreateAsync(category);

            return CreatedAtAction(nameof(Get), new { id = category.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ToothBasic val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var category = await _toothService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _toothService.UpdateAsync(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _toothService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _toothService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("GetAllDisplay")]
        public async Task<IActionResult> GetAllDisplay(ToothFilter val)
        {
            var res = await _toothService.GetAllDisplay(val);
            return Ok(res);
        }
    }
}