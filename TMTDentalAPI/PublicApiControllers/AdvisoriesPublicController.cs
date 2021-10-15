using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.PublicApiControllers
{
    [Route("publicApi/Advisories")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class AdvisoriesPublicController : ControllerBase
    {
        private readonly IAdvisoryService _advisoryService;
        private readonly IMapper _mapper;
        public AdvisoriesPublicController(IAdvisoryService advisoryService, IMapper mapper)
        {
            _advisoryService = advisoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] AdvisoryPublicFilter val)
        {
            var advisories = await _advisoryService.GetAdvisoriesByPartnerId(val.PartnerId);
            var res = _mapper.Map<IEnumerable<AdvisoryPublicReponse>>(advisories);
            return Ok(res);
        }
    }
}
