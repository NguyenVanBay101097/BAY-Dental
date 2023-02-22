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
    public class SaleSettingsController : BaseApiController
    {
        private readonly ISaleSettingsService _saleSettingService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public SaleSettingsController(ISaleSettingsService saleSettingService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _saleSettingService = saleSettingService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var setting = await _saleSettingService.GetSettings();
            var display = _mapper.Map<SaleSettingsDisplay>(setting);
            return Ok(display);
        }

        [HttpPut]
        public async Task<IActionResult> Update(SaleSettingsDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var setting = await _saleSettingService.GetSettings();
            setting = _mapper.Map(val, setting);
            await _saleSettingService.UpdateAsync(setting);
            return NoContent();
        }
    }
}