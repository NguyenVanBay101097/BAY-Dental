using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionProgramsController : BaseApiController
    {
        private readonly IPromotionProgramService _programService;
        private readonly IMapper _mapper;

        public PromotionProgramsController(IPromotionProgramService programService,
            IMapper mapper)
        {
            _programService = programService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]PromotionProgramPaged val)
        {
            var result = await _programService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var display = await _programService.GetDisplay(id);
            if (display == null)
                return NotFound();

            return Ok(display);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PromotionProgramSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var program = await _programService.CreateProgram(val);

            var basic = _mapper.Map<PromotionProgramBasic>(program);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PromotionProgramSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _programService.UpdateProgram(id, val);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var program = await _programService.GetByIdAsync(id);
            if (program == null)
                return NotFound();
            await _programService.DeleteAsync(program);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ToggleActive(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _programService.ToggleActive(ids);
            return NoContent();
        }
    }
}