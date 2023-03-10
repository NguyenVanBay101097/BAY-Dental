using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
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
        private readonly IIrConfigParameterService _irConfigParameterService;
        private readonly IImportSampleDataService _importSampleDataService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly CatalogDbContext _dbContext;
        private readonly IResGroupService _groupService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppTenant _tenant;

        public WebController(
            IIrAttachmentService attachmentService,
            IMapper mapper,
            IUploadService uploadService,
            IIrConfigParameterService irConfigParameterService,
            IImportSampleDataService importSampleDataService,
            IUnitOfWorkAsync unitOfWork,
            IUserService userService,
            ICompanyService companyService,
             CatalogDbContext dbContext,
             IResGroupService groupService, IWebHostEnvironment webHostEnvironment,
             ITenant<AppTenant> tenant)
        {
            _attachmentService = attachmentService;
            _mapper = mapper;
            _uploadService = uploadService;
            _irConfigParameterService = irConfigParameterService;
            _importSampleDataService = importSampleDataService;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _companyService = companyService;
            _dbContext = dbContext;
            _groupService = groupService;
            _webHostEnvironment = webHostEnvironment;
            _tenant = tenant?.Value;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ImportSampleData([FromQuery] string action)
        {
            //Cancel / bo qua
            //Installed / import
            if (action == "Installed")
            {
                var configSampleData = await _irConfigParameterService.GetParam("import_sample_data");
                if (string.IsNullOrEmpty(configSampleData))
                {
                    await _unitOfWork.BeginTransactionAsync();
                    await _importSampleDataService.ImportSampleData();
                    await _irConfigParameterService.SetParam("import_sample_data", action);
                    _unitOfWork.Commit();
                }
            }
            else
            {
                await _irConfigParameterService.SetParam("import_sample_data", action);
            }

            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DeleteSampleData()
        {
            var configSampleData = await _irConfigParameterService.GetParam("import_sample_data");
            if (!string.IsNullOrEmpty(configSampleData) && configSampleData == "Installed")
            {
                await _unitOfWork.BeginTransactionAsync();
                await _importSampleDataService.DeleteSampleData();
                _unitOfWork.Commit();
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<IActionResult> OldSaleOrderPaymentProcessUpdate()
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _importSampleDataService.OldSaleOrderPaymentProcessUpdate();
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw e;
            }

            return NoContent();
        }



        [HttpPost("[action]")]
        public async Task<IActionResult> BinaryUploadAttachment([FromForm] UploadAttachmentViewModel val)
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
                    Type = "url",
                    Url = item.FileUrl,
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

     
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var result = await _HandleUploadAsync(Convert.ToBase64String(memoryStream.ToArray()), file.FileName);
                return Ok(result);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadImageByBase64(UploadBinaryViewModel data)
        {

            var result = await _uploadService.UploadImageByBase64Async(data.Uri);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadImages(IList<IFormFile> files)
        {
            var result = await _uploadService.UploadBinaryAsync(files);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult Features()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"SampleData\features.json");
            using (var reader = new StreamReader(filePath))
            {
                var fileContent = reader.ReadToEnd();
                var features = JsonConvert.DeserializeObject<List<PermissionTreeViewModel>>(fileContent);

                return Ok(features);
            }
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult GetExpire()
        {
            if (_tenant == null || !_tenant.DateExpired.HasValue) return Ok();
            TimeSpan diff = _tenant.DateExpired.Value - DateTime.Now;
            string res = string.Format(
                                   "{0} ngày {1} giờ {2} phút",
                                   diff.Days,
                                   Math.Abs(diff.Hours),
                                   Math.Abs(diff.Minutes));
            return Ok(new { ExpireText = res, ExpireDate = _tenant.DateExpired.Value });
        }
    }
}