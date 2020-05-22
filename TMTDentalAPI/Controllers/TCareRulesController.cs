using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TCareRulesController : BaseApiController
    {
        private readonly ITCareRuleService _ruleService;
        private readonly IMapper _mapper;

        public TCareRulesController(ITCareRuleService ruleService,
            IMapper mapper)
        {
            _ruleService = ruleService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _ruleService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TCareRuleSave val)
        {
            var rule = await _ruleService.CreateRule(val);
            var basic = _mapper.Map<TCareRuleBasic>(rule);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TCareRuleSave val)
        {
            await _ruleService.UpdateRule(id, val);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var rule = await _ruleService.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            await _ruleService.DeleteAsync(rule);
            return NoContent();
        }
    }
}