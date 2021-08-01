using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsBirthdayAutomationConfigsController : BaseApiController
    {
        private readonly ISmsBirthdayAutomationConfigService _smsConfigService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public SmsBirthdayAutomationConfigsController(IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsBirthdayAutomationConfigService smsConfigService)
        {
            _smsConfigService = smsConfigService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        [HttpGet]
        [CheckAccess(Actions = "SMS.Config.Read")]
        public async Task<IActionResult> Get()
        {
            var res = await _smsConfigService.GetByCompany();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "SMS.Config.Create")]
        public async Task<IActionResult> SaveConfig(SmsBirthdayAutomationConfigSave val)
        {
            var entity = await _smsConfigService.SearchQuery(x => x.CompanyId == CompanyId).FirstOrDefaultAsync();
            await _unitOfWorkAsync.BeginTransactionAsync();

            if (entity != null)
            {
                entity = _mapper.Map(val, entity);
                await _smsConfigService.UpdateAsync(entity);
            }
            else
            {
                entity = _mapper.Map<SmsBirthdayAutomationConfig>(val);
                entity = await _smsConfigService.CreateAsync(entity);
            }
            _unitOfWorkAsync.Commit();


            return NoContent();
        }
    }
}
