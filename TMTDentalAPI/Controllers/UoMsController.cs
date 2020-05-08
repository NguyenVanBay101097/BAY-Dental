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
            var uom = _mapper.Map<UoM>(val);
            if (val.UOMType == "bigger")
                uom.Factor = val.FactorInv.HasValue && val.FactorInv.Value != 0 ? 1 / val.FactorInv.Value : 0;
            var res = await _uoMService.CreateAsync(uom);

            var basic = _mapper.Map<UoMBasic>(res);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UoMSave val)
        {
            var uom = await _uoMService.GetByIdAsync(id);
            if (uom == null)
                return NotFound();

            uom = _mapper.Map(val, uom);

            if (val.UOMType == "bigger")
                uom.Factor = val.FactorInv.HasValue && val.FactorInv.Value != 0 ? 1 / val.FactorInv.Value : 0;

            await _uoMService.UpdateAsync(uom);

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
            res.FactorInv = res.Factor != 0 ? 1 / res.Factor : 0;
            return Ok(res);
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] UoMPaged val)
        {
            var res = await _uoMService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(UoMPaged val)
        {
            var res = await _uoMService.GetAutocompleteAsync(val);
            return Ok(res);
        }

    }
}