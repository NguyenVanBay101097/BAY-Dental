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
    public class HrSalaryRulesController : BaseApiController
    {

        private readonly IHrSalaryRuleService _HrSalaryRuleService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrSalaryRulesController(IHrSalaryRuleService HrSalaryRuleService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _HrSalaryRuleService = HrSalaryRuleService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] HrSalaryRulePaged val)
        {
            var res = await _HrSalaryRuleService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var HrSalaryRule = await _HrSalaryRuleService.GetHrSalaryRuleDisplay(id);
            if (HrSalaryRule == null)
                return NotFound();
            var res = _mapper.Map<HrSalaryRuleDisplay>(HrSalaryRule);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrSalaryRuleSave val)
        {
            var entitys = _mapper.Map<HrSalaryRule>(val);

            await _unitOfWork.BeginTransactionAsync();
            await _HrSalaryRuleService.CreateAsync(entitys);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<HrSalaryRuleDisplay>(entitys));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrSalaryRuleSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _HrSalaryRuleService.GetHrSalaryRuleDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);

            await _HrSalaryRuleService.UpdateAsync(str);

            return NoContent();
        }
      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var HrSalaryRule = await _HrSalaryRuleService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (HrSalaryRule == null)
            {
                return NotFound();
            }
            await _HrSalaryRuleService.DeleteAsync(HrSalaryRule);
            return NoContent();
        }
    }
}
