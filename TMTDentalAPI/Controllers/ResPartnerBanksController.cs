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
    public class ResPartnerBanksController : BaseApiController
    {
        private IResPartnerBankService _resPartnerBankService;
        private IMapper _mapper;

        public ResPartnerBanksController(IResPartnerBankService resPartnerBankService, IMapper mapper)
        {
            _mapper = mapper;
            _resPartnerBankService = resPartnerBankService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ResPartnerBankPaged selfPaged)
        {
            var res = await _resPartnerBankService.GetPagedResultAsync(selfPaged);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _resPartnerBankService.GetByIdAsync(id);
            return Ok(_mapper.Map<ResPartnerBankBasic>(res));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResPartnerBankDisplay self)
        {
            if (null == self || !ModelState.IsValid)
                return BadRequest();
            var resBank = _mapper.Map<ResPartnerBank>(self);
            var res = await _resPartnerBankService.CreateAsync(resBank);
            return CreatedAtAction(nameof(Get), new { id = res.Id }, self);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ResPartnerBankDisplay self)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var bank = await _resPartnerBankService.GetByIdAsync(id);
            if (bank == null)
                return NotFound();

            bank = _mapper.Map(self, bank);
            await _resPartnerBankService.UpdateAsync(bank);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var bank = await _resPartnerBankService.GetByIdAsync(id);
            if (bank == null)
                return NotFound();

            await _resPartnerBankService.DeleteAsync(bank);

            return NoContent();
        }
    }
}