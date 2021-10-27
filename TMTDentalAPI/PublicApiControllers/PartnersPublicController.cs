using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.Controllers;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.PublicApiControllers
{
    [Route("publicApi/Partners")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class PartnersPublicController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        public PartnersPublicController(IHttpContextAccessor httpContextAccessor, IPartnerService partnerService, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _partnerService = partnerService;
            _mapper = mapper;
        }

       
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PublicPartnerRequest val)
        {
            var query = _partnerService.SearchQuery(x => x.Active && x.Customer);

            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search) || x.NameNoSign.Contains(val.Search)
              || x.Ref.Contains(val.Search) || x.Phone.Contains(val.Search));
            }

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await _mapper.ProjectTo<PublicPartnerReponse>(query.OrderByDescending(x => x.DateCreated)).ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var query = _partnerService.SearchQuery(x => x.Id == id);
            var res = await _mapper.ProjectTo<PublicPartnerInfo>(query).FirstOrDefaultAsync();
            return Ok(res);
        }
    }
}
