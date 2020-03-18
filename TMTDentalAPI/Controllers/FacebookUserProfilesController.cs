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
            var lstfbpage = await _facebookUserProfileService.GetPagedResultAsync(val);
            if (lstfbpage == null)
            {
                return NotFound();
            }
            return Ok(lstfbpage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var fbuser = await _facebookUserProfileService.GetByIdAsync(id);
            if (fbuser == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<FacebookUserProfileBasic>(fbuser));
        }

        

        [HttpPost("[action]")]
        public async Task<IActionResult> ConnectPartner(ConnectPartner val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var fbCus = await _facebookUserProfileService.ActionConectPartner(val);
           
            return NoContent();
        }
        //[HttpPost]
        //public async Task<IActionResult> SearchSender(Fanpage val )
        //{
        //    if (null == val || !ModelState.IsValid)
        //        return BadRequest();

        //    var senders = await _facebookUserProfileService.GetListPSId(val);
        //    //var basic = _mapper.Map<Face>(senders);
        //    return Ok();
        //}
    }
}