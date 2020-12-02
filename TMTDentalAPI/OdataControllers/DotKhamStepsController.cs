using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class DotKhamStepsController : ControllerBase
    {
        private readonly IDotKhamStepService _dotKhamStepService;
        private readonly IMapper _mapper;

        public DotKhamStepsController(IMapper mapper, IDotKhamStepService dotKhamStepService)
        {
            _dotKhamStepService = dotKhamStepService;
            _mapper = mapper;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var results = _mapper.ProjectTo<DotmKhamStepVM>(_dotKhamStepService.SearchQuery());
            return Ok(results);
        }

        [EnableQuery]
        [HttpGet]
        public SingleResult<DotmKhamStepVM> Get([FromODataUri] Guid key)
        {
            var results = _mapper.ProjectTo<DotmKhamStepVM>(_dotKhamStepService.SearchQuery(x => x.Id == key));
            return SingleResult.Create(results);
        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, ActionDotKham val)
        {
            var dotkham = await _dotKhamStepService.GetByIdAsync(key);
            if (dotkham == null)
                return NotFound();
            dotkham.IsDone = val.IsDone;
            await _dotKhamStepService.UpdateAsync(dotkham);
            return NoContent();
        }

        
    }
}
