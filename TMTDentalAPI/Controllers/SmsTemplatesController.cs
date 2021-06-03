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
    public class SmsTemplatesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISmsTemplateService _smsTemplateService;

        public SmsTemplatesController(IMapper mapper, ISmsTemplateService smsTemplateService)
        {
            _mapper = mapper;
            _smsTemplateService = smsTemplateService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SmsTemplateSave val)
        {
            var entity = _mapper.Map<SmsTemplate>(val);
            entity.CompanyId = CompanyId;
            entity = await _smsTemplateService.CreateAsync(entity);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsTemplateSave val)
        {
            var entity = await _smsTemplateService.GetByIdAsync(id);
            if (!ModelState.IsValid || entity == null)
                return BadRequest();
            entity = _mapper.Map(val, entity);
            entity.CompanyId = CompanyId;
            await _smsTemplateService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsTemplateService.GetByIdAsync(id);
            return Ok(_mapper.Map<SmsTemplateBasic>(res));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SmsTemplatePaged val)
        {
            var res = await _smsTemplateService.GetPaged(val);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _smsTemplateService.GetByIdAsync(id);
            if (res == null) return NotFound();
            await _smsTemplateService.DeleteAsync(res);
            return NoContent();
        }
        
        [HttpGet("Autocomplete")]
        public async Task<IActionResult> GetTemplateAutocomplete([FromQuery] SmsTemplateFilter val)
        {
            var res = await _smsTemplateService.GetTemplateAutocomplete(val);
            return Ok(res);
        }
    }
}
