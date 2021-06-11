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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsAccountsController : BaseApiController
    {
        private readonly ISmsAccountService _smsAccountService;
        private readonly IMapper _mapper;
        public SmsAccountsController(IMapper mapper, ISmsAccountService smsAccountService)
        {
            _smsAccountService = smsAccountService;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SMS.Account.Read")]
        public async Task<IActionResult> GetPaged([FromQuery] SmsAccountPaged val)
        {
            var res = await _smsAccountService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "SMS.Account.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsAccountService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();

            return Ok(_mapper.Map<SmsAccountDisplay>(res));
        }
     
        [HttpPost]
        [CheckAccess(Actions = "SMS.Account.Create")]
        public async Task<IActionResult> CreateAsync(SmsAccountSave val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();
            var entity = _mapper.Map<SmsAccount>(val);
            entity.CompanyId = CompanyId;
            entity = await _smsAccountService.CreateAsync(entity);
            var res = _mapper.Map<SmsAccountBasic>(entity);
            return Ok(res);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "SMS.Account.Update")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsAccountSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = await _smsAccountService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
          
            entity = _mapper.Map(val, entity);
            entity.CompanyId = CompanyId;
            await _smsAccountService.UpdateAsync(entity);
            var res = _mapper.Map<SmsAccountBasic>(entity);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "SMS.Account.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _smsAccountService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            await _smsAccountService.DeleteAsync(entity);
            return NoContent();
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SMS.Account.Read")]
        public async Task<IActionResult> SmsSupplierAutocomplete(string search)
        {
            var res = await _smsAccountService.SmsSupplierAutocomplete(search);
            return Ok(res);
        }
    }
}
