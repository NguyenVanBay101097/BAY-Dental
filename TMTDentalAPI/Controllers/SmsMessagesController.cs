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
    public class SmsMessagesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISmsMessageService _smsMessageService;

        public SmsMessagesController(IMapper mapper, ISmsMessageService smsMessageService)
        {
            _smsMessageService = smsMessageService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SmsMessageSave val)
        {
            if (!ModelState.IsValid || val == null) return BadRequest();
            var entity = _mapper.Map<SmsMessage>(val);
            entity = await _smsMessageService.CreateAsync(entity);
            var res = _mapper.Map<SmsAccountBasic>(entity);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsMessageService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            return Ok(_mapper.Map<SmsAccountDisplay>(res));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPaged([FromQuery] SmsMessagePaged val)
        {
            var res = await _smsMessageService.GetPaged(val);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsAccountSave val)
        {
            var entity = await _smsMessageService.GetByIdAsync(id);
            if (!ModelState.IsValid || entity == null) return BadRequest();
            entity = _mapper.Map(val, entity);
            await _smsMessageService.UpdateAsync(entity);
            var res = _mapper.Map<SmsAccountBasic>(entity);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _smsMessageService.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return NoContent();
        }
    }
}
