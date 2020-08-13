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
    public class HrSalaryRuleCategoryController : BaseApiController
    {

        private readonly IHrSalaryRuleCategoryService _HrSalaryRuleCategoryService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrSalaryRuleCategoryController(IHrSalaryRuleCategoryService HrSalaryRuleCategoryService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _HrSalaryRuleCategoryService = HrSalaryRuleCategoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] HrSalaryRuleCategoryPaged val)
        {
            var res = await _HrSalaryRuleCategoryService.GetPaged(val);
            return Ok(res);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(Guid id)
        //{
        //    var HrSalaryRuleCategory = await _HrSalaryRuleCategoryService.GetHrSalaryRuleCategoryDisplay(id);
        //    if (HrSalaryRuleCategory == null)
        //        return NotFound();
        //    var res = _mapper.Map<HrSalaryRuleCategoryDisplay>(HrSalaryRuleCategory);
        //    return Ok(res);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(HrSalaryRuleCategorySave val)
        //{
        //    var entitys = _mapper.Map<HrSalaryRuleCategory>(val);

        //    await _HrSalaryRuleCategoryService.CreateAsync(entitys);
        //    val.Id = entitys.Id;
        //    return Ok(val);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(Guid id, HrSalaryRuleCategorySave val)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest();
        //    var str = await _HrSalaryRuleCategoryService.GetHrSalaryRuleCategoryDisplay(id);
        //    if (str == null)
        //        return NotFound();

        //    str = _mapper.Map(val, str);

        //    await _HrSalaryRuleCategoryService.UpdateAsync(str);

        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var HrSalaryRuleCategory = await _HrSalaryRuleCategoryService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (HrSalaryRuleCategory == null)
            {
                return NotFound();
            }
            await _HrSalaryRuleCategoryService.DeleteAsync(HrSalaryRuleCategory);
            return NoContent();
        }

    }
}
