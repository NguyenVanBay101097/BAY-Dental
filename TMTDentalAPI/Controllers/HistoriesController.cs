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
    public class HistoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _service;

        public HistoriesController(IMapper mapper, IHistoryService service)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]HistoryPaged val)
        {
            var result = await _service.GetPagedResultAsync(val);
            var paged = new PagedResult2<HistorySimple>(result.TotalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HistorySimple>>(result.Items)
            };
            return Ok(paged);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            var entity = _mapper.Map<HistorySimple>(result);
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HistorySimple val)
        {
            if (val == null)
                return BadRequest();
            var dup = await _service.CheckDuplicate(Guid.Empty,val);
            if (dup)
                throw new Exception("Bệnh này đã tồn tại !");

            var history = _mapper.Map<History>(val);
            await _service.CreateAsync(history);
            val.Id = history.Id;

            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HistorySimple val)
        {
            var result = await _service.GetByIdAsync(id);
            if(result==null)
                return NotFound();

            var dup = await _service.CheckDuplicate(id,val);
            if (dup)
                throw new Exception("Bệnh này đã tồn tại !");

            result = _mapper.Map(val,result);
            await _service.UpdateAsync(result);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            await _service.DeleteAsync(result);

            return NoContent();
        }

        [HttpGet("GetHistoriesCheckbox")]
        public async Task<IActionResult> GetHistoriesCheckbox()
        {
            var val = new HistoryPaged();
            var result = await _service.GetResultNotLimitAsync(val);
            var entity = _mapper.Map<IEnumerable<HistorySimple>>(result);
            return Ok(entity);
        }
    }
}