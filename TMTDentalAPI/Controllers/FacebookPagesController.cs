using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
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
        private readonly IUnitOfWorkAsync _unitOfWork;

        public FacebookPagesController(IMapper mapper, IFacebookPageService facebookPageService, IPartnerService partnerService, IFacebookUserProfileService facebookUserProfileService,
            IUserService userService, IUnitOfWorkAsync unitOfWork) 
        {
            _mapper = mapper;
            _facebookPageService = facebookPageService;
            _partnerService = partnerService;
            _facebookUserProfileService = facebookUserProfileService;
            _userService = userService;
            _unitOfWork = unitOfWork;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var page = await _facebookPageService.GetByIdAsync(id);
            if (page == null)
                return NotFound();
            await _facebookPageService.DeleteAsync(page);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SyncUsers(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _facebookPageService.SyncUsers(ids);
            _unitOfWork.Commit();

            return NoContent();
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
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAutoConfigAppointment()
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var fbpage = await _facebookPageService._GetAutoConfig();           
            return Ok(fbpage);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateAutoConfigAppointment(FacebookScheduleAppointmentConfigSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await  _facebookPageService.SaveAutoConfig(val);
            return Ok();
        }

       
    }
}