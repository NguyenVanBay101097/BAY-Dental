using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    public class ResBanksController : BaseApiController
    {
        private IResBankService _resBankService;
        private IMapper _mapper;

        public ResBanksController(IResBankService resBankService, IMapper mapper)
        {
            _mapper = mapper;
            _resBankService = resBankService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ResBankPaged selfPaged)
        {
            var res = await _resBankService.GetPagedResultAsync(selfPaged);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _resBankService.GetByIdAsync(id);
            return Ok(_mapper.Map<ResBankBasic>(res));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResBankBasic self)
        {
            if (null == self || !ModelState.IsValid)
                return BadRequest();
            var resBank = _mapper.Map<ResBank>(self);
            var res = await _resBankService.CreateAsync(resBank);
            return CreatedAtAction(nameof(Get), new { id = res.Id }, self);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResBankBasic self)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var bank = await _resBankService.GetByIdAsync(id);
            if (bank == null)
                return NotFound();

            bank = _mapper.Map(self,bank);
            await _resBankService.UpdateAsync(bank);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var bank = await _resBankService.GetByIdAsync(id);
            if (bank == null)
                return NotFound();

            await _resBankService.DeleteAsync(bank);

            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Autocomplete([FromQuery]ResPartnerBankPaged val)
        {
            var res = await _resBankService.AutocompleteAsync(val);
            return Ok(res);
        }
    }
}