using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class PartnersController : BaseController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;

        public PartnersController(IPartnerService partnerService,
            IMapper mapper)
        {
            _partnerService = partnerService;
            _mapper = mapper;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] IEnumerable<Guid> tagIds)
        {
            var results = await _partnerService.GetViewModelsAsync();
            if (tagIds != null && tagIds.Any())
                results = results.Where(x => x.Tags.Any(s => tagIds.Contains(s.Id)));
            return Ok(results);
        }

        [HttpPut]
        public IActionResult Put([FromODataUri]Guid key, PartnerViewModel value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(value);
            }
           
            return NoContent();
        }

        [EnableQuery]
        [HttpGet("[action]")]
        public IActionResult GetView()
        {
            return Ok(true);
        }
    }
}
