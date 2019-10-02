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
    public class DotKhamLinesController : BaseApiController
    {
        private readonly IDotKhamLineService _dotKhamLineService;
        private readonly IMapper _mapper;
        private readonly IIRModelAccessService _modelAccessService;

        public DotKhamLinesController(IDotKhamLineService dotKhamLineService,
            IMapper mapper, IIRModelAccessService modelAccessService)
        {
            _dotKhamLineService = dotKhamLineService;
            _modelAccessService = modelAccessService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _modelAccessService.Check("DotKhamLine", "Read");
            var line = await _dotKhamLineService.SearchQuery(x => x.Id == id).Include(x => x.Product)
                .Include(x => x.User).Include(x => x.Operations)
                .Include("Operations.Product")
                .Include(x => x.Routing).FirstOrDefaultAsync();
            if (line == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DotKhamLineDisplay>(line));
        }

        [HttpPost]
        public async Task<IActionResult> Create(DotKhamLineDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("DotKhamLine", "Create");
            var category = _mapper.Map<DotKhamLine>(val);
            await _dotKhamLineService.CreateAsync(category);

            return CreatedAtAction(nameof(Get), new { id = category.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, DotKhamLineDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("DotKhamLine", "Write");
            var category = await _dotKhamLineService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _dotKhamLineService.UpdateAsync(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            _modelAccessService.Check("DotKhamLine", "Unlink");
            var category = await _dotKhamLineService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _dotKhamLineService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("{id}/MarkDone")]
        public async Task<IActionResult> MarkDone(Guid id)
        {
            await _dotKhamLineService.MarkDone(new List<Guid>(){ id });
            return NoContent();
        }

        [HttpPost("ChangeRouting")]
        public async Task<IActionResult> ChangeRouting(DotKhamLineChangeRouting val)
        {
            await _dotKhamLineService.ChangeRouting(val);
            return NoContent();
        }
    }
}