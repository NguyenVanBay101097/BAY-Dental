﻿using System;
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
        private readonly IUploadService _uploadService;

        public WebController(IIrAttachmentService attachmentService,
            IMapper mapper, IUploadService uploadService)
        {
            _attachmentService = attachmentService;
            _mapper = mapper;
            _uploadService = uploadService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> BinaryUploadAttachment([FromForm]UploadAttachmentViewModel val)
        {
            if (val == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var list = new List<IrAttachment>();

            var tasks = new List<Task<UploadResult>>();
            foreach (var file in val.files)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    tasks.Add(_HandleUploadAsync(Convert.ToBase64String(memoryStream.ToArray()), file.FileName));
                }
            }

            var result = await Task.WhenAll(tasks);
            foreach (var item in result)
            {
                if (item == null)
                    throw new Exception("Hình không hợp lệ hoặc vượt quá kích thước cho phép.");
                var attachment = new IrAttachment()
                {
                    ResModel = val.model,
                    ResId = val.id,
                    Name = item.Name,
                    Type = "upload",
                    UploadId = item.Id,
                    MineType = item.MineType,
                    CompanyId = CompanyId
                };

                list.Add(attachment);
            }

            await _attachmentService.CreateAsync(list);

            var res = _mapper.Map<IEnumerable<IrAttachmentBasic>>(list);
            return Ok(res);
        }

        private Task<UploadResult> _HandleUploadAsync(string base64, string fileName)
        {
            return _uploadService.UploadBinaryAsync(base64, fileName: fileName);
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