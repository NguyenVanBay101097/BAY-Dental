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
    public class PartnerMapPSIDFacebookPageController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IFacebookPageService _facebookPageService;
        private readonly IPartnerService _partnerService;
        private readonly IPartnerMapPSIDFacebookPageService _partnerMapPSIDFacebookPageService;
        public PartnerMapPSIDFacebookPageController(IMapper mapper, IFacebookPageService facebookPageService, IPartnerService partnerService, IPartnerMapPSIDFacebookPageService partnerMapPSIDFacebookPageService)
        {
            _mapper = mapper;
            _facebookPageService = facebookPageService;
            _partnerService = partnerService;
            _partnerMapPSIDFacebookPageService = partnerMapPSIDFacebookPageService;
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var mapfbpage = await _partnerMapPSIDFacebookPageService.GetByIdAsync(id);
            if (mapfbpage == null)
            {
                return NotFound();
            }
            var partner = _partnerService.GetById(mapfbpage.PartnerId);
            var basic = new PartnerMapPSIDFacebookPageBasic
            {
                id = mapfbpage.Id,
                PartnerId = partner.Id,
                PartnerName = partner.Name,
                PartnerEmail = partner.Email,
                PartnerPhone = partner.Phone
            };
            return Ok(basic);
        }


        [HttpPost]
        public async Task<IActionResult> CreatePartnerForFacebookPage(CreatePartner val)
        {

            var result = await _partnerMapPSIDFacebookPageService.CreatePartnerMapFBPage(val);
            if (result == null)
            {
                return NotFound();
            }
            var partner = _partnerService.GetById(result.PartnerId);
            var basic = new PartnerMapPSIDFacebookPageBasic
            {
                id = result.Id,
                PartnerId = partner.Id,
                PartnerName = partner.Name,
                PartnerEmail = partner.Email,
                PartnerPhone = partner.Phone
            };
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> MergePartnerForFacebookPage(CheckPartnerMapFBPage val)
        {

            var result = await _partnerMapPSIDFacebookPageService.MergePartnerMapFBPage(val);

            if (result == null)
            {
                return NotFound();
            }
            var partner = _partnerService.GetById(result.PartnerId);
            var basic = new PartnerMapPSIDFacebookPageBasic
            {
                id = result.Id,
                PartnerId = partner.Id,
                PartnerName = partner.Name,
                PartnerEmail = partner.Email,
                PartnerPhone = partner.Phone
            };
            return Ok(basic);


        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CheckPartnerFBPage(CheckPartnerMapFBPage val)
        {
            var result = await _partnerMapPSIDFacebookPageService.CheckPartnerMergeFBPage(val.PartnerId, val.PageId, val.PSId);
            if (result != null)
            {
                throw new Exception(" khách hàng đã liên kết !");
            }

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnchangePartner (string phone)
        {
            var result = await _partnerService.OnChangePartner(phone);
            return Ok(result);
        }

    }
}