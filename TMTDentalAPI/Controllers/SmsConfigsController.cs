using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
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
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public SmsConfigsController(IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsConfigService smsConfigService)
        {
            _smsConfigService = smsConfigService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        [HttpGet]
        public async Task<IActionResult> GetDefault()
        {
            var res = await _smsConfigService.SearchQuery(x => x.CompanyId == CompanyId)
                .Include(x => x.AppointmentTemplate)
                .Include(x => x.BirthdayTemplate)
                .FirstOrDefaultAsync();
            return Ok(_mapper.Map<SmsConfigBasic>(res));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(SmsConfigSave val)
        {
            var entity = _mapper.Map<SmsConfig>(val);
            entity.CompanyId = CompanyId;
            await _unitOfWorkAsync.BeginTransactionAsync();
            entity = await _smsConfigService.CreateAsync(entity);
            _unitOfWorkAsync.Commit();
            return Ok(_mapper.Map<SmsConfigBasic>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsConfigSave val)
        {
            var entity = await _smsConfigService.GetByIdAsync(id);
            if (entity == null || !ModelState.IsValid)
                return NotFound();
            entity = _mapper.Map(val, entity);
            entity.CompanyId = CompanyId;
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsConfigService.UpdateAsync(entity);
            _unitOfWorkAsync.Commit();
            return Ok(_mapper.Map<SmsConfigBasic>(entity));
        }


    }
}
