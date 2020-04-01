using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
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
    public class FacebookMassMessagingsController : ControllerBase
    {
        private readonly IFacebookMassMessagingService _facebookMassMessagingService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public FacebookMassMessagingsController(IFacebookMassMessagingService facebookMassMessagingService,
            IMapper mapper, IUserService userService, IUnitOfWorkAsync unitOfWork)
        {
            _facebookMassMessagingService = facebookMassMessagingService;
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FacebookMassMessagingPaged val)
        {
            var res = await _facebookMassMessagingService.GetPagedResultAsync(val);
            return Ok(res);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var messaging = await _facebookMassMessagingService.SearchQuery(x => x.Id == id)
                .Include(x => x.Traces).FirstOrDefaultAsync();
            if (messaging == null)
                return NotFound();

            var display = _mapper.Map<FacebookMassMessagingDisplay>(messaging);
            return Ok(display);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacebookMassMessagingSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var messaging = _mapper.Map<FacebookMassMessaging>(val);
            await _facebookMassMessagingService.CreateAsync(messaging);

            var basic = _mapper.Map<FacebookMassMessagingBasic>(messaging);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, FacebookMassMessagingSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var messaging = await _facebookMassMessagingService.GetByIdAsync(id);
            if (messaging == null)
                return NotFound();

            messaging = _mapper.Map(val, messaging);
            await _facebookMassMessagingService.UpdateAsync(messaging);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _facebookMassMessagingService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGet()
        {
            var user = await _userService.GetCurrentUser();
            if (!user.FacebookPageId.HasValue)
                throw new Exception("Tài khoản này chưa làm việc với page nào.");
            return Ok(new FacebookMassMessagingDisplay { FacebookPageId = user.FacebookPageId.Value });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionSend(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _facebookMassMessagingService.ActionSend(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SetScheduleDate(FacebookMassMessagingSetScheduleDate val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _facebookMassMessagingService.SetScheduleDate(val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _facebookMassMessagingService.ActionCancel(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> ActionViewDelivered(Guid id, [FromQuery]FacebookMassMessagingStatisticsPaged paged)
        {
            var res = await _facebookMassMessagingService.ActionViewDelivered(id, paged);
            return Ok(res);
        }
    }
}