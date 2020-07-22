using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TCareScenariosController : ControllerBase
    {
        private readonly ITCareScenarioService _scenarioService;
        private readonly IMapper _mapper;
        public TCareScenariosController(ITCareScenarioService scenarioService, IMapper mapper)
        {
            _scenarioService = scenarioService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]TCareScenarioPaged paged)
        {
            var res = await _scenarioService.GetPagedResultAsync(paged);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _scenarioService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Autocomplete(TCareScenarioPaged val)
        {
            var res = await _scenarioService.GetAutocompleteAsync(val);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TCareScenarioSave val)
        {
            var model = _mapper.Map<TCareScenario>(val);
            var res = await _scenarioService.CreateAsync(model);
            var display = _mapper.Map<TCareScenarioDisplay>(res);
            return Ok(display);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, TCareScenarioSave val)
        {
            var model = await _scenarioService.GetByIdAsync(id);
            model.Name = val.Name;
            await _scenarioService.UpdateAsync(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var model = await _scenarioService.GetByIdAsync(id);
            await _scenarioService.DeleteAsync(model);
            return NoContent();
        }
    }
}