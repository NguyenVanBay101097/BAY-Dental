using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class IRSequencesController : ControllerBase
    {
        private readonly IIRSequenceService _iRSequenceService;
        private readonly IMapper _mapper;
        public IRSequencesController(
            IIRSequenceService iRSequenceService,
            IMapper mapper
            )
        {
            _iRSequenceService = iRSequenceService;
            _mapper = mapper;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get(string code)
        {
            var results = await _iRSequenceService.GetViewModelsAsync();
            return Ok(results);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, IRSequenceSave val)
        {
            var model = await _iRSequenceService.GetByIdAsync(id);
            if (model == null || val == null || !ModelState.IsValid)
                return BadRequest();
            model.Prefix = val.Prefix;
            model.NumberNext = val.NumberNext;
            model.Padding = val.Padding;
            model.NumberIncrement = val.NumberIncrement;
            await _iRSequenceService.UpdateAsync(model);
            return NoContent();
        }
    }
}
