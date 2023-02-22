using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class FacebookUserProfilesController : BaseController
    {
        private readonly IFacebookUserProfileService _facebookUserProfileService;
        private readonly IMapper _mapper;
        private readonly IPartnerService _partnerService;
        public FacebookUserProfilesController(IFacebookUserProfileService facebookUserProfileService,
            IMapper mapper, IPartnerService partnerService)
        {
            _facebookUserProfileService = facebookUserProfileService;
            _mapper = mapper;
            _partnerService = partnerService;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var results = _facebookUserProfileService.SearchQuery();
            return Ok(results);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromODataUri] Guid key, JsonPatchDocument<FacebookUserProfile> patch)
        {
            var profile = await _facebookUserProfileService.GetByIdAsync(key);
            if (profile == null)
                return NotFound();

            patch.ApplyTo(profile);
            await _facebookUserProfileService.UpdateAsync(profile);
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetView(ODataQueryOptions<FacebookUserProfileBasic> options)
        {
            var results = _mapper.ProjectTo<FacebookUserProfileBasic>(_facebookUserProfileService.SearchQuery());
            results = options.ApplyTo(results) as IQueryable<FacebookUserProfileBasic>;
            var list = await results.ToListAsync();

            var partnerIds = list.Where(x => x.PartnerId.HasValue).Select(x => x.PartnerId.Value).Distinct().ToList();
            var partners = await _mapper.ProjectTo<PartnerSimple>(_partnerService.SearchQuery(x => partnerIds.Contains(x.Id))).ToListAsync();
            var partnerDict = partners.ToDictionary(x => x.Id, x => x);

            foreach (var item in list)
            {
                var partner = item.PartnerId.HasValue && partnerDict.ContainsKey(item.PartnerId.Value) ? partnerDict[item.PartnerId.Value] : null;
                item.Partner = partner != null ? JsonConvert.SerializeObject(partner) : "";
            }

            return Ok(list);
        }
    }
}
