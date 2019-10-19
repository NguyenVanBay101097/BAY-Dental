using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IRRulesController : BaseApiController
    {
        private readonly IIRRuleService _ruleService;
        private readonly IMapper _mapper;
        public IRRulesController(IIRRuleService ruleService,
            IMapper mapper)
        {
            _ruleService = ruleService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int offset = 0,
           int limit = 10, string filter = "")
        {
            var paged = await _ruleService.GetPagedAsync(offset: offset, limit: limit, filter: filter);
            var res = new PagedResult2<IRRuleBasic>(paged.TotalItems, paged.Offset, paged.Limit)
            {
                Items = _mapper.Map<IEnumerable<IRRuleBasic>>(paged.Items)
            };

            return Ok(res);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _ruleService.SearchQuery(x => x.Id == id).Include(x => x.Model).FirstOrDefaultAsync();
            if (model == null)
            {
                return NotFound();
            }
            var display = _mapper.Map<IRRuleDisplay>(model);
            return Ok(display);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IRRuleDisplay val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            var rule = _mapper.Map<IRRule>(val);

            await _ruleService.CreateAsync(rule);
            val.Id = rule.Id;

            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, IRRuleDisplay val)
        {
            var model = await _ruleService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            model = _mapper.Map(val, model);

            await _ruleService.UpdateAsync(model);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _ruleService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            await _ruleService.DeleteAsync(model);
            return NoContent();
        }
    }
}