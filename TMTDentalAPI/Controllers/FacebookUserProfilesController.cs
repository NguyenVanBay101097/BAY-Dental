using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookUserProfilesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPartnerService _partnerService;
        private readonly IFacebookUserProfileService _facebookUserProfileService;
        public FacebookUserProfilesController(IMapper mapper, IPartnerService partnerService , IFacebookUserProfileService facebookUserProfileService )
        {
            _mapper = mapper;         
            _partnerService = partnerService;
            _facebookUserProfileService = facebookUserProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]FacebookUserProfilePaged val)
        {
            var res = await _facebookUserProfileService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var profile = await _facebookUserProfileService.SearchQuery(x => x.Id == id)
                .Include(x => x.Partner).FirstOrDefaultAsync();
            
            if (profile == null)
                return NotFound();

            return Ok(_mapper.Map<FacebookUserProfileDisplay>(profile));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, FacebookUserProfileSave val)
        {
            await _facebookUserProfileService.UpdateUserProfile(id, val);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConnectPartner(ConnectPartner val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var fbCus = await _facebookUserProfileService.ActionConectPartner(val);
           
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemovePartner(IEnumerable<Guid> ids)
        {
            if (null == ids || !ModelState.IsValid)
                return BadRequest();
            await _facebookUserProfileService.ActionRemovePartner(ids);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveUserProfile(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();            
            await _facebookUserProfileService.RemoveUserProfile(ids);        
            return NoContent();
        }
    }
}