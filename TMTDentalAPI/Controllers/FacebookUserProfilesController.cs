using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookUserProfilesController : ControllerBase
    {
        private readonly IFacebookUserProfileService _userProfileService;
        private readonly IMapper _mapper;

        public FacebookUserProfilesController(IFacebookUserProfileService userProfileService,
            IMapper mapper)
        {
            _userProfileService = userProfileService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var profile = await _userProfileService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (profile == null)
                return NotFound();

            return Ok(_mapper.Map<FacebookUserProfileDisplay>(profile));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument<FacebookUserProfile> patch)
        {
            var entity = await _userProfileService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            patch.ApplyTo(entity);
            await _userProfileService.UpdateAsync(entity);

            return NoContent();
        }
    }
}