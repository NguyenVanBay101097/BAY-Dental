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
    public class UoMsController : ControllerBase
    {
        private readonly IUoMService _uoMService;
        private readonly IMapper _mapper;
        public UoMsController(IUoMService uoMService, IMapper mapper)
        {
            _uoMService = uoMService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(UoMSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var type = _mapper.Map<UoM>(val);
            var res = await _uoMService.CreateAsync(type);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UoMSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var type = await _uoMService.GetByIdAsync(id);
            if (type == null)
                return NotFound();

            type = _mapper.Map(val, type);
            await _uoMService.UpdateAsync(type);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _uoMService.GetByIdAsync(id);
            if (type == null)
                return NotFound();
            await _uoMService.DeleteAsync(type);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var type = await _uoMService.SearchQuery(x => x.Id == id).Include(x => x.Category)
                .FirstOrDefaultAsync();
            if (type == null)
                return NotFound();

            var res = _mapper.Map<UoMDisplay>(type);
            return Ok(res);
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] UoMPaged val)
        {
            var res = await _uoMService.GetPagedResultAsync(val);
            return Ok(res);
        }

    }
}