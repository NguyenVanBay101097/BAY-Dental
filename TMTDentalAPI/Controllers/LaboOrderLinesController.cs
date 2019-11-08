using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaboOrderLinesController : BaseApiController
    {
        private readonly ILaboOrderLineService _laboOrderLineService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRModelAccessService _modelAccessService;

        public LaboOrderLinesController(ILaboOrderLineService laboOrderLineService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IIRModelAccessService modelAccessService)
        {
            _laboOrderLineService = laboOrderLineService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _modelAccessService = modelAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] LaboOrderLinePaged val)
        {
            var res = await _laboOrderLineService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var laboLine = await _laboOrderLineService.GetLaboLineForDisplay(id);
            if (laboLine == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<LaboOrderLineDisplay>(laboLine));
        }

        [HttpPost]
        public async Task<IActionResult> Create(LaboOrderLineDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var laboLine = _mapper.Map<LaboOrderLine>(val);
            await _laboOrderLineService.CreateAsync(laboLine);

            val.Id = laboLine.Id;
            return CreatedAtAction(nameof(Get), new { id = laboLine.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, LaboOrderLineDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var laboLine = await _laboOrderLineService.GetByIdAsync(id);
            if (laboLine == null)
                return NotFound();

            laboLine = _mapper.Map(val, laboLine);
            await _laboOrderLineService.UpdateAsync(laboLine);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var laboLine = await _laboOrderLineService.GetByIdAsync(id);
            if (laboLine == null)
                return NotFound();
            await _laboOrderLineService.DeleteAsync(laboLine);

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(LaboOrderLineDefaultGet val)
        {
            var res = await _laboOrderLineService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("OnChangeProduct")]
        public async Task<IActionResult> OnChangeProduct(LaboOrderLineOnChangeProduct val)
        {
            var res = await _laboOrderLineService.OnChangeProduct(val);
            return Ok(res);
        }
    }
}