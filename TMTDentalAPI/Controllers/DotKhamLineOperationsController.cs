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
    public class DotKhamLineOperationsController : BaseApiController
    {
        private readonly IDotKhamLineOperationService _dotKhamLineOperationService;
        private readonly IMapper _mapper;

        public DotKhamLineOperationsController(IDotKhamLineOperationService dotKhamLineOperationService,
            IMapper mapper)
        {
            _dotKhamLineOperationService = dotKhamLineOperationService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _dotKhamLineOperationService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DotKhamLineOperationDisplay>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(DotKhamLineOperationDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<DotKhamLineOperation>(val);
            await _dotKhamLineOperationService.CreateAsync(category);

            return CreatedAtAction(nameof(Get), new { id = category.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, DotKhamLineOperationDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var category = await _dotKhamLineOperationService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _dotKhamLineOperationService.UpdateAsync(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _dotKhamLineOperationService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _dotKhamLineOperationService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("{id}/MarkDone")]
        public async Task<IActionResult> MarkDone(Guid id)
        {
            await _dotKhamLineOperationService.MarkDone(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("{id}/StartOperation")]
        public async Task<IActionResult> StartOperation(Guid id)
        {
            await _dotKhamLineOperationService.StartOperation(id);
            return NoContent();
        }
    }
}