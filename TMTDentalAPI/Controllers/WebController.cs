using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.ViewModels;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebController : BaseApiController
    {
        private readonly IIrAttachmentService _attachmentService;
        private readonly IMapper _mapper;
        public WebController(IIrAttachmentService attachmentService,
            IMapper mapper)
        {
            _attachmentService = attachmentService;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> BinaryUploadAttachment([FromForm]UploadAttachmentViewModel val)
        {
            if (val == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var companyId = CompanyId;
            var list = new List<IrAttachment>();
            foreach (var file in val.files)
            {
                var fileName = file.FileName;

                var attachment = new IrAttachment
                {
                    Name = fileName,
                    DatasFname = fileName,
                    ResModel = val.model,
                    ResId = val.id,
                    MineType = file.ContentType,
                    CompanyId = companyId
                };

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    attachment.DbDatas = memoryStream.ToArray();
                }

                await _attachmentService.CreateAsync(attachment);
                list.Add(attachment);
            }

            var res = _mapper.Map<IEnumerable<IrAttachmentBasic>>(list);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        [HttpGet("[action]/{id}/{width}x{height}")]
        public async Task<IActionResult> Image(Guid id, int width, int height, bool crop = false)
        {
            var attachment = await _attachmentService.GetByIdAsync(id);
            if (attachment == null)
                return NotFound();

            var content = attachment.DbDatas;
            if (crop && (width != 0 || height != 0))
            {
                content = Convert.FromBase64String(ImageHelper.CropImage(Convert.ToBase64String(content), type: "center", width: width, height: height));
            }
            else if (width != 0 || height != 0)
            {
                content = Convert.FromBase64String(ImageHelper.ImageResizeImage(content, width: width, height: height));
            }
            return new FileContentResult(content, attachment.MineType);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> Image(string model = "ir.attachment", Guid? id = null, string field = "datas")
        {
            var attachment = await _attachmentService.GetAttachment(model, id, field);
            var content = attachment.DbDatas;
            return File(content, attachment.MineType);
        }

        [AllowAnonymous]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Content(Guid id, bool download = false)
        {
            var attachment = await _attachmentService.GetByIdAsync(id);

            var content = attachment.DbDatas;
            Response.ContentType = attachment.MineType;
            var fileName = System.Web.HttpUtility.UrlEncode(attachment.DatasFname);
            Response.Headers.Add("Content-Disposition", String.Format("attachment;filename*=UTF-8\"{0}\"", fileName));
            return File(content, attachment.MineType, attachment.DatasFname);
        }
    }
}