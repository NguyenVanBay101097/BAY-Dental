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
using TMTDentalAPI.JobFilters;
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

        //[HttpGet]
        //public async Task<IActionResult> GetDefault()
        //{
        //    var res = await _smsConfigService.SearchQuery(x => x.CompanyId == CompanyId)
        //        .Include(x => x.Template)
        //        .FirstOrDefaultAsync();
        //    return Ok(_mapper.Map<SmsConfigBasic>(res));
        //}

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SMS.Config.Read")]
        public async Task<IActionResult> GetPaged(SmsConfigPaged val)
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

        [HttpGet]
        [CheckAccess(Actions = "SMS.Config.Read")]
        public async Task<IActionResult> Get(string type)
        {
            var res = await _smsConfigService.SearchQuery(x => x.CompanyId == CompanyId && x.Type == type)
                .Include(x => x.Template)
                .Include(x => x.SmsAccount)
                .FirstOrDefaultAsync();
            return Ok(_mapper.Map<SmsConfigBasic>(res));
        }

        [HttpPost]
        [CheckAccess(Actions = "SMS.Config.Create")]
        public async Task<IActionResult> CreateAsync(SmsConfigSave val)
        {
            var entity = _mapper.Map<SmsConfig>(val);
            entity.CompanyId = CompanyId;
            if (val.ProductCategoryIds.Any())
            {
                foreach (var id in val.ProductCategoryIds)
                {
                    entity.SmsConfigProductCategoryRels.Add(new SmsConfigProductCategoryRel
                    {
                        ProductCategoryId = id
                    });
                }
            }
            else if (val.ProductIds.Any())
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
            return Ok(_mapper.Map<SmsConfigBasic>(entity));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "SMS.Config.Update")]
        public async Task<IActionResult> UpdateAsync(Guid id, SmsConfigSave val)
        {
            var entity = await _smsConfigService.GetByIdAsync(id);
            if (entity == null || !ModelState.IsValid)
                return NotFound();
            await _unitOfWorkAsync.BeginTransactionAsync();
            await _smsConfigService.UpdateAsync(id, val);
            _unitOfWorkAsync.Commit();
            return Ok(_mapper.Map<SmsConfigBasic>(entity));
        }


    }
}
