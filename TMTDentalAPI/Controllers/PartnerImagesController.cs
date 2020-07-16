using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.ViewModels;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerImagesController : ControllerBase
    {
        private readonly IUploadService _uploadService;
        private readonly IPartnerImageService _partnerImageService;
        private readonly IMapper _mapper;
        private readonly IPartnerService _partnerService;

        public PartnerImagesController(
            IUploadService uploadService,
            IPartnerImageService partnerImageService,
            IPartnerService partnerService,
            IMapper mapper
            )
        {
            _partnerImageService = partnerImageService;
            _uploadService = uploadService;
            _mapper = mapper;
            _partnerService = partnerService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> BinaryUploadPartnerImage([FromForm]UploadPartnerImageViewModel val)
        {
            var partner = await _partnerService.GetByIdAsync(val.PartnerId);

            if (val == null || !ModelState.IsValid || partner == null)
            {
                return BadRequest();
            }

            var res = await _partnerImageService.BinaryUploadPartnerImage(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchRead(PartnerImageSearchRead val)
        {
            var res = await _partnerImageService.SearchRead(val);
            return Ok(res);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var partnerImage = await _partnerImageService.GetByIdAsync(id);
            if (partnerImage == null)
                return NotFound();
            await _partnerImageService.DeleteAsync(partnerImage);
            return NoContent();
        }
    }
}