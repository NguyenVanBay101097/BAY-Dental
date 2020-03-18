using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookPagesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IFacebookPageService _facebookPageService;
        private readonly IPartnerService _partnerService;
        private readonly IFacebookUserProfileService _facebookUserProfileService;
        public FacebookPagesController(IMapper mapper, IFacebookPageService facebookPageService, IPartnerService partnerService, IFacebookUserProfileService facebookUserProfileService) {
            _mapper = mapper;
            _facebookPageService = facebookPageService;
            _partnerService = partnerService;
            _facebookUserProfileService = facebookUserProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]FacebookPaged val)
        {
            var lstfbpage = await _facebookPageService.GetPagedResultAsync(val);
            if (lstfbpage == null)
            {
                return NotFound();
            }
            return Ok(lstfbpage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var fbpage = await _facebookPageService.GetByIdAsync(id);
            if (fbpage == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<FacebookPageBasic>(fbpage));
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacebookPageLinkSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var fbpage = await _facebookPageService.CreateFacebookPage(val);
            var basic = _mapper.Map<FacebookPageBasic>(fbpage);
            return Ok(basic);
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> CreateFacebookUser(Guid id)
        {
            if (null == id || !ModelState.IsValid)
                return BadRequest();

            var fbpage = await _facebookUserProfileService.CreateFacebookUser(id);
            var basic = _mapper.Map<List<FacebookUserProfileBasic>>(fbpage);
            return Ok(basic);
        }

      
    }
}