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
    public class FacebookPagesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IFacebookPageService _facebookPageService;
        private readonly IUserService _userService;
        private readonly IPartnerService _partnerService;
        private readonly IFacebookUserProfileService _facebookUserProfileService;
        public FacebookPagesController(IMapper mapper, IFacebookPageService facebookPageService, IPartnerService partnerService, IFacebookUserProfileService facebookUserProfileService,
            IUserService userService) {
            _mapper = mapper;
            _facebookPageService = facebookPageService;
            _partnerService = partnerService;
            _facebookUserProfileService = facebookUserProfileService;
            _userService = userService;
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

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateFacebookUser()
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var fbpage = await _facebookPageService.CreateFacebookUser();
            var basic = _mapper.Map<List<FacebookUserProfileBasic>>(fbpage);
            return Ok(basic);
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> SelectPage(Guid id)
        {
            var user = await _userService.GetCurrentUser();
            user.FacebookPageId = id;
            await _userService.UpdateAsync(user);

            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSwitchPage()
        {
            var user = await _userService.GetCurrentUser();
            var model = new UserChangeCurrentFacebookPage();
            if (user.FacebookPageId.HasValue)
            {
                var currentPage = await _facebookPageService.GetByIdAsync(user.FacebookPageId);
                model.CurrentPage = _mapper.Map<FacebookPageBasic>(currentPage);
            }

            var pages = await _facebookPageService.SearchQuery().ToListAsync();
            model.Pages = _mapper.Map<IEnumerable<FacebookPageBasic>>(pages);

            return Ok(model);
        }
    }
}