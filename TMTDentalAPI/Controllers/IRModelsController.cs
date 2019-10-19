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
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IRModelsController : BaseApiController
    {
        private readonly IIRModelService _modelService;
        private readonly IMapper _mapper;
        public IRModelsController(IIRModelService modelService,
            IMapper mapper)
        {
            _modelService = modelService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int offset = 0,
           int limit = 10, string filter = "")
        {
            var paged = await _modelService.GetPagedAsync(offset: offset, limit: limit, filter: filter);
            var res = new PagedResult2<IRModelBasic>(paged.TotalItems, paged.Offset, paged.Limit)
            {
                Items = _mapper.Map<IEnumerable<IRModelBasic>>(paged.Items)
            };

            return Ok(res);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _modelService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            var display = _mapper.Map<IRModelDisplay>(model);
            return Ok(display);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IRModelSave val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            var model = _mapper.Map<IRModel>(val);

            await _modelService.CreateAsync(model);

            var display = _mapper.Map<IRModelDisplay>(model);
            return Ok(display);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, IRModelSave val)
        {
            var model = await _modelService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            model = _mapper.Map(val, model);

            await _modelService.UpdateAsync(model);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _modelService.GetByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            await _modelService.DeleteAsync(model);
            return NoContent();
        }
    }
}