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
    public class SmsConfigsController : BaseApiController
    {
        private readonly ISmsConfigService _smsConfigService;
        private readonly IMapper _mapper;
        public SmsConfigsController(IMapper mapper, ISmsConfigService smsConfigService)
        {
            _smsConfigService = smsConfigService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetDefault()
        {
            var res = await _smsConfigService.SearchQuery().Include(x => x.AppointmentTemplate).Include(x => x.BirthdayTemplate).FirstOrDefaultAsync();
            return Ok(_mapper.Map<SmsConfigBasic>(res));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SmsConfigSave val)
        {
            var entity = _mapper.Map<SmsConfig>(val);
            entity = await _smsConfigService.CreateAsync(entity);
            return Ok(_mapper.Map<SmsConfigBasic>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsConfigSave val)
        {
            var entity = await _smsConfigService.GetByIdAsync(id);
            if (entity == null || !ModelState.IsValid)
                return NotFound();
            entity = _mapper.Map(val, entity);
            await _smsConfigService.UpdateAsync(entity);
            return Ok(_mapper.Map<SmsConfigBasic>(entity));
        }


    }
}
