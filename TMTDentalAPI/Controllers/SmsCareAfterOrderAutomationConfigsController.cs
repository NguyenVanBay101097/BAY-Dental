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
    public class SmsCareAfterOrderAutomationConfigsController : BaseApiController
    {
        private readonly ISmsCareAfterOrderAutomationConfigService _smsConfigService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public SmsCareAfterOrderAutomationConfigsController(IUnitOfWorkAsync unitOfWorkAsync, IMapper mapper, ISmsCareAfterOrderAutomationConfigService smsConfigService)
        {
            _smsConfigService = smsConfigService;
            _mapper = mapper;
            _unitOfWorkAsync = unitOfWorkAsync;
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "SMS.Config.Read")]
        public async Task<IActionResult> GetPaged(SmsCareAfterOrderAutomationConfigPaged val)
        {
            var res = await _smsConfigService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "SMS.Config.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _smsConfigService.GetDisplay(id);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "SMS.Config.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _smsConfigService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();
            await _smsConfigService.DeleteAsync(entity);
            return NoContent();
        }

        [HttpPost]
        [CheckAccess(Actions = "SMS.Config.Create")]
        public async Task<IActionResult> CreateAsync(SmsCareAfterOrderAutomationConfigSave val)
        {
            var entity = _mapper.Map<SmsCareAfterOrderAutomationConfig>(val);
            entity.CompanyId = CompanyId;
            if (entity.ApplyOn == "product_category")
            {
                foreach (var id in val.ProductCategoryIds)
                {
                    entity.SmsConfigProductCategoryRels.Add(new SmsConfigProductCategoryRel
                    {
                        ProductCategoryId = id
                    });
                }
            }
            else if (entity.ApplyOn == "product")
            {
                foreach (var id in val.ProductIds)
                {
                    entity.SmsConfigProductRels.Add(new SmsConfigProductRel
                    {
                        ProductId = id
                    });
                }
            }
          
            await _unitOfWorkAsync.BeginTransactionAsync();
            entity = await _smsConfigService.CreateAsync(entity);
            _unitOfWorkAsync.Commit();

            return Ok(_mapper.Map<SmsCareAfterOrderAutomationConfigDisplay>(entity));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "SMS.Config.Update")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsCareAfterOrderAutomationConfigSave val)
        {
            var entity = await _smsConfigService.GetByIdAsync(id);
            if (entity == null || !ModelState.IsValid)
                return NotFound();
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsConfigService.UpdateAsync(id, val);
            _unitOfWorkAsync.Commit();
            return NoContent();
        }
    }
}
