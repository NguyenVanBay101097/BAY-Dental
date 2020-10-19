using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
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
        public async Task<IActionResult> Get(ODataQueryOptions<PartnerViewModel> options, [FromQuery] IEnumerable<Guid> tagIds)
        {
            var results = await _partnerService.GetViewModelsAsync();
            var a = options.ApplyTo(results) as IQueryable<PartnerViewModel>;
            var b = a.ToList();

            //timf nhan cua 10 phan tu trong a
            foreach(var item in b)
            {
            }

            if (tagIds != null && tagIds.Any())
                results = results.Where(x => x.Tags.Any(s => tagIds.Contains(s.Id)));
            return Ok(a);
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
        public async Task<IActionResult> GetView(ODataQueryOptions<PartnerViewModel> options)
        {
            var results = await _partnerService.GetViewModelsAsync();

            var a = options.ApplyTo(results) as IQueryable<PartnerViewModel>;
            var b = a.ToList();

            //timf nhan cua 10 phan tu trong a
            foreach (var item in b)
            {
            }

            return Ok(b);
        }
    }
}
