using System;
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
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IPartnerCategoryService _partnerCategoryService;
        private readonly IUoMService _uoMService;
        private readonly IPartnerService _partnerService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly IProductStepService _productStepService;
        private readonly IHistoryService _historyService;

        public WebController(
            IIrAttachmentService attachmentService,
            IMapper mapper,
            IUploadService uploadService,
            IIrConfigParameterService irConfigParameterService,
            IPartnerCategoryService partnerCategoryService,
            IUoMService uoMService,
            IPartnerService partnerService,
            IProductCategoryService productCategoryService,
            IProductService productService,
            IProductStepService productStepService,
            IHistoryService historyService
            )
        {
            _attachmentService = attachmentService;
            _mapper = mapper;
            _uploadService = uploadService;
            _irConfigParameterService = irConfigParameterService;
            _partnerCategoryService = partnerCategoryService;
            _uoMService = uoMService;
            _partnerService = partnerService;
            _productCategoryService = productCategoryService;
            _productService = productService;
            _productStepService = productStepService;
            _historyService = historyService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ImportSimpleData([FromQuery]string question)
        {
            await _irConfigParameterService.SetParam("import_simple_data", "False");
            if (question == "yes")
            {
                var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"sampleData.xml");
                XElement xml = XElement.Load(path);
                XmlSerializer serializer = new XmlSerializer(typeof(Tdental));
                MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xml.ToString()));
                Tdental tdental = (Tdental)serializer.Deserialize(memStream);
                if (tdental != null && tdental.Data != null && tdental.Data.Record.Count > 0)
                {

                    var record = tdental.Data.Record;

                    foreach (var itemRecord in record.ToList())
                    {
                        switch (itemRecord.Model)
                        {
                            case "res.partner.category":
                                PartnerCategory partnerCategory = new PartnerCategory();
                                foreach (var itemField in itemRecord.Field.ToList())
                                {
                                    switch (itemField.Name)
                                    {
                                        case "name":
                                            partnerCategory.Name = itemField.Text;
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                await _partnerCategoryService.CreateAsync(partnerCategory);
                                break;
                            case "res.partner":
                                Partner partner = new Partner();
                                foreach (var itemField in itemRecord.Field.ToList())
                                {
                                    switch (itemField.Name)
                                    {
                                        case "name":
                                            partner.Name = itemField.Text;
                                            break;
                                        case "email":
                                            partner.Email = itemField.Text;
                                            break;
                                        case "phone":
                                            partner.Phone = itemField.Text;
                                            break;
                                        case "gender":
                                            partner.Gender = itemField.Text;
                                            break;
                                        case "birth_day":
                                            partner.BirthDay = Int32.Parse(itemField.Text);
                                            break;
                                        case "birth_month":
                                            partner.BirthMonth = Int32.Parse(itemField.Text);
                                            break;
                                        case "birth_year":
                                            partner.BirthYear = Int32.Parse(itemField.Text);
                                            break;
                                        case "supplier":
                                            partner.Supplier = Boolean.Parse(itemField.Text);
                                            break;
                                        case "customer":
                                            partner.Customer = Boolean.Parse(itemField.Text);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                await _partnerService.CreateAsync(partner);

                                break;
                            case "product.category":
                                ProductCategory productCateg = new ProductCategory();
                                foreach (var itemField in itemRecord.Field.ToList())
                                {
                                    switch (itemField.Name)
                                    {
                                        case "name":
                                            productCateg.Name = itemField.Text;
                                            break;
                                        case "type":
                                            productCateg.Type = itemField.Text;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                await _productCategoryService.CreateAsync(productCateg);

                                var listProductRecord = record.Where(x => x.Field.Any(s => s.Ref == itemRecord.Id)).ToList();
                                var uom = await _uoMService.SearchQuery(x => x.Name.Contains("Cái")).FirstOrDefaultAsync();
                                foreach (var productRecord in listProductRecord)
                                {
                                    Product product = new Product();
                                    product.UOMId = uom.Id;
                                    product.UOMPOId = uom.Id;
                                    product.CompanyId = CompanyId;
                                    foreach (var itemField in productRecord.Field.ToList())
                                    {
                                        switch (itemField.Name)
                                        {
                                            case "name":
                                                product.Name = itemField.Text;
                                                break;
                                            case "list_price":
                                                product.ListPrice = Decimal.Parse(itemField.Text);
                                                break;
                                            case "labo_price":
                                                product.LaboPrice = Decimal.Parse(itemField.Text);
                                                break;
                                            case "purchase_price":
                                                product.PurchasePrice = Decimal.Parse(itemField.Text);
                                                break;
                                            case "is_labo":
                                                product.IsLabo = Boolean.Parse(itemField.Text);
                                                break;
                                            case "purchase_ok":
                                                product.PurchaseOK = Boolean.Parse(itemField.Text);
                                                break;
                                            case "type":
                                                product.Type = itemField.Text;
                                                break;
                                            case "type_2":
                                                product.Type2 = itemField.Text;
                                                break;
                                            case "categ_id":
                                                product.CategId = productCateg.Id;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    await _productService.CreateAsync(product);
                                    var listProductStepRecord = record.Where(x => x.Model == "product.step" && x.Field.Any(s => s.Ref == productRecord.Id)).ToList();
                                    foreach (var productStepRecord in listProductStepRecord)
                                    {
                                        ProductStep productStep = new ProductStep();
                                        foreach (var itemField in productStepRecord.Field.ToList())
                                        {
                                            if (itemField.Name == "name")
                                                productStep.Name = itemField.Text;
                                            if (itemField.Name == "product_id")
                                                productStep.ProductId = product.Id;
                                        }
                                        await _productStepService.CreateAsync(productStep);
                                    }
                                }
                                break;
                            case "history":
                                History history = new History();
                                foreach (var itemField in itemRecord.Field.ToList())
                                {
                                    switch (itemField.Name)
                                    {
                                        case "name":
                                            history.Name = itemField.Text;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                await _historyService.CreateAsync(history);
                                break;
                            default:
                                break;
                        }

                    }

                }
            }
            return NoContent();

        }

        [HttpGet("[action]/{key}")]
        public async Task<IActionResult> GetIrConfigParameter(string key)
        {
            var res = await _irConfigParameterService.GetParam(key);
            if (res == null)
                return BadRequest();
            return Ok(new { res });
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
    }
}