using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
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
            IMapper mapper)
        {
            _iRSequenceService = iRSequenceService;
            _mapper = mapper;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _iRSequenceService.GetViewModelsAsync();
            return Ok(results);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromODataUri] Guid key, IRSequenceSave val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            var sequence = await _iRSequenceService.GetByIdAsync(key);
            if (sequence == null)
                return NotFound();

            sequence.Prefix = val.Prefix;
            sequence.NumberNext = val.NumberNext;
            sequence.Padding = val.Padding;
            await _iRSequenceService.UpdateAsync(sequence);
            return NoContent();
        }
    }
}
