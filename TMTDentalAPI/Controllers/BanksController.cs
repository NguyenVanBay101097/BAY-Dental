using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanksController : BaseApiController
    {
        private readonly IBankService _bankService;
        private readonly IMapper _mapper;

        public BanksController(IBankService bankService, IMapper mapper)
        {
            _bankService = bankService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] BankPaged val)
        {
            var res = await _bankService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var bank = await _bankService.GetByIdAsync(id);
            if (bank == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BankBasic>(bank));
        }

        [HttpPost]
        public async Task<IActionResult> Create(BankSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var bank = _mapper.Map<Bank>(val);
            var res = await _bankService.CreateAsync(bank);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, BankSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var bank = await _bankService.GetByIdAsync(id);
            if (bank == null)
                return NotFound();

            bank = _mapper.Map(val, bank);
            await _bankService.UpdateAsync(bank);
            return NoContent();
        }

        [HttpDelete("{id}")]     
        public async Task<IActionResult> Remove(Guid id)
        {
            var bank = await _bankService.GetByIdAsync(id);
            if (bank == null)
                return NotFound();
            await _bankService.DeleteAsync(bank);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(BankPaged val)
        {
            var banks = await _bankService.GetAutocompleteAsync(limit: val.Limit , offset: val.Offset , search: val.Search);
            var res = _mapper.Map<IEnumerable<BankBasic>>(banks); 
            return Ok(res);
        }
    }
}
