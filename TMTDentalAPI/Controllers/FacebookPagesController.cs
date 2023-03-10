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
using TMTDentalAPI.JobFilters;
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
        [CheckAccess(Actions = "TCare.Channel.Read")]
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
        [CheckAccess(Actions = "TCare.Channel.Read")]
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
        [CheckAccess(Actions = "TCare.Channel.Create")]
        public async Task<IActionResult> Create(FacebookPageLinkSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var fbpage = await _facebookPageService.CreateFacebookPage(val);
            var basic = _mapper.Map<FacebookPageBasic>(fbpage);
            return Ok(basic);
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "TCare.Channel.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var page = await _facebookPageService.GetByIdAsync(id);
            if (page == null)
                return NotFound();
            await _facebookPageService.DeleteAsync(page);

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "TCare.Channel.Update")]
        public async Task<IActionResult> SyncUsers(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _facebookPageService.SyncUsers(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "TCare.Channel.Update")]
        public async Task<IActionResult> SyncNumberPhoneUsers(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _facebookPageService.SyncNumberPhoneUsers(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SyncPhoneForMultiUsers(MultiUserProfilesVm val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _facebookPageService.SyncPhoneForMultiUsers(val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "TCare.Channel.Update")]
        public async Task<IActionResult> SyncPartnersForNumberPhone(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _facebookPageService.SyncPartnersForNumberPhone(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SyncPartnersForMultiUser(MultiUserProfilesVm val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _facebookPageService.SyncPartnersForMultiUsers(val);
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
        [CheckAccess(Actions = "TCare.Channel.Read")]
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "TCare.Channel.update")]
        public async Task<IActionResult> RefreshSocialChannel(FacebookPageSimple val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _facebookPageService.RefreshSocial(val);
            return Ok();
        }

    }
}