using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.Controllers;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.PublicApiControllers
{
    [Route("publicApi/[controller]")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class PartnersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        public PartnersController(IHttpContextAccessor httpContextAccessor, IPartnerService partnerService, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _partnerService = partnerService;
            _mapper = mapper;
        }

       
        [HttpGet]
        public async Task<IActionResult> Get([FromBody] PublicPartnerRequest val)
        {
            var partners = await _partnerService.GetPublicPartners(val.Limit, val.Offset, val.Search);
            var res = _mapper.Map<IEnumerable<PublicPartnerReponse>>(partners);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var partner = await _partnerService.GetPartnerForDisplayAsync(id);
            var res = _mapper.Map<PublicPartnerInfo>(partner);
            return Ok(res);
        }
    }
}
