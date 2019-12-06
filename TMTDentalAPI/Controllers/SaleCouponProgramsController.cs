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
    public class SaleCouponProgramsController : BaseApiController
    {
        private readonly ISaleCouponProgramService _programService;
        private readonly IMapper _mapper;

        public SaleCouponProgramsController(ISaleCouponProgramService programService,
            IMapper mapper)
        {
            _programService = programService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]SaleCouponProgramPaged val)
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
        public async Task<IActionResult> Create(SaleCouponProgramSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var program = await _programService.CreateProgram(val);

            var basic = _mapper.Map<SaleCouponProgramBasic>(program);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SaleCouponProgramSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _programService.UpdateProgram(id, val);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _programService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateCoupons(SaleCouponProgramGenerateCoupons val)
        {
            await _programService.GenerateCoupons(val);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _programService.Unlink(ids);
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