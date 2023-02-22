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
        private readonly IUnitOfWorkAsync _unitOfWork;
        public PartnerMapPSIDFacebookPageController(IMapper mapper, IFacebookPageService facebookPageService, IPartnerService partnerService, IPartnerMapPSIDFacebookPageService partnerMapPSIDFacebookPageService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _facebookPageService = facebookPageService;
            _partnerService = partnerService;
            _partnerMapPSIDFacebookPageService = partnerMapPSIDFacebookPageService;
            _unitOfWork = unitOfWork;
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
        public async Task<IActionResult> CreatePartnerForFacebookPage(PartnerMapPSIDFacebookPageSave val)
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

        //[HttpPost("[action]")]
        //public async Task<IActionResult> MergePartnerForFacebookPage(PartnerMapPSIDFacebookPageSave val)
        //{

        //    var result = await _partnerMapPSIDFacebookPageService.MergePartnerMapFBPage(val);

        //    if (result == null)
        //    {
        //        return NotFound();
        //    }
        //    var partner = _partnerService.GetById(result.PartnerId);
        //    var basic = new PartnerMapPSIDFacebookPageBasic
        //    {
        //        id = result.Id,             
        //        PartnerName = partner.Name,
        //        PartnerEmail = partner.Email,
        //        PartnerPhone = partner.Phone
        //    };
        //    return Ok(basic);


        //}

        [HttpPost("[action]")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid>ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _partnerMapPSIDFacebookPageService.Unlink(ids);
            _unitOfWork.Commit();          
                     
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnchangePartner (string phone)
        {
            await _unitOfWork.BeginTransactionAsync();
            var result = await _partnerService.OnChangePartner(phone);
            _unitOfWork.Commit();           
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CheckPartner(CheckPartner val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var result = await _partnerMapPSIDFacebookPageService.CheckPartner(val);
            _unitOfWork.Commit();
                    
            return Ok(result);
        }


    }
}