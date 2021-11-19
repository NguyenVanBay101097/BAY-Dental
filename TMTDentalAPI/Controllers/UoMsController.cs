using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UoMsController : ControllerBase
    {
        private readonly IUoMService _uoMService;
        private readonly IUoMCategoryService _uoMCategService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public UoMsController(IUoMService uoMService, IMapper mapper,
            IUoMCategoryService uoMCategService,
            IUnitOfWorkAsync unitOfWork)
        {
            _uoMService = uoMService;
            _mapper = mapper;
            _uoMCategService = uoMCategService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [CheckAccess(Actions = "UoM.UoMs.Create")]
        public async Task<IActionResult> Create(UoMSave val)
        {
            var uom = _mapper.Map<UoM>(val);

            if (val.UOMType == "bigger")
                uom.Factor = val.FactorInv.HasValue && val.FactorInv.Value != 0 ? 1 / val.FactorInv.Value : 0;

            var res = await _uoMService.CreateAsync(uom);

            var basic = _mapper.Map<UoMBasic>(res);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "UoM.UoMs.Update")]
        public async Task<IActionResult> Update(Guid id, UoMSave val)
        {
            var uom = await _uoMService.SearchQuery(x => x.Id == id).Include(x => x.Category).FirstOrDefaultAsync();
            if (uom == null)
                return NotFound();

            uom = _mapper.Map(val, uom);

            if (val.UOMType == "bigger")
                uom.Factor = val.FactorInv.HasValue && val.FactorInv.Value != 0 ? 1 / val.FactorInv.Value : 0;

            await _uoMService.UpdateAsync(uom);

            return NoContent();
        }


        [HttpDelete("{id}")]
        [CheckAccess(Actions = "UoM.UoMs.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _uoMService.GetByIdAsync(id);
            if (type == null)
                return NotFound();
            await _uoMService.DeleteAsync(type);
            return NoContent();
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "UoM.UoMs.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var type = await _uoMService.SearchQuery(x => x.Id == id).Include(x => x.Category)
                .FirstOrDefaultAsync();
            if (type == null)
                return NotFound();

            var res = _mapper.Map<UoMDisplay>(type);
            res.FactorInv = res.Factor != 0 ? 1 / res.Factor : 0;
            return Ok(res);
        }


        [HttpGet]
        [CheckAccess(Actions = "UoM.UoMs.Read")]
        public async Task<IActionResult> Get([FromQuery] UoMPaged val)
        {
            var res = await _uoMService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "UoM.UoMs.Create")]
        public async Task<IActionResult> ImportExcel(ProductImportExcelViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<UoMImportExcelRow>();
            var errors = new List<string>();

            var typeDict = new Dictionary<string, string>()
            {
                { "Là đơn vị gốc của nhóm này", "reference" },
                { "Lớn hơn đơn vị gốc", "bigger" },
                { "Nhỏ hơn đơn vị gốc", "smaller" }
            };

            try
            {
                using (var stream = new MemoryStream(fileData))
                {
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var errs = new List<string>();
                            var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                            var categName = Convert.ToString(worksheet.Cells[row, 2].Value);
                            var type = Convert.ToString(worksheet.Cells[row, 3].Value);
                            var factor = Convert.ToDecimal(worksheet.Cells[row, 4].Value);

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên đơn vị là bắt buộc");
                            if (string.IsNullOrEmpty(categName))
                                errs.Add("Nhóm đơn vị là bắt buộc");
                            if (string.IsNullOrEmpty(type))
                                errs.Add("Loại là bắt buộc");

                            if (factor == 0)
                                errs.Add("Tỉ lệ phải khác 0");

                            if (!string.IsNullOrEmpty(type) && !typeDict.ContainsKey(type))
                                errs.Add($"Loại không hợp lệ. Giá trị cho phép là {string.Join(", ", typeDict.Keys.ToArray())}");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }

                            var item = new UoMImportExcelRow
                            {
                                Name = name,
                                Type = type,
                                CategName = categName,
                                Factor = factor
                            };
                            data.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng" } });
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var categNames = data.Select(x => x.CategName).Distinct().ToList();
            var categs = await _uoMCategService.SearchQuery(x => categNames.Contains(x.Name)).ToListAsync();
            var categDict = categs.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var notFoundCategNames = categNames.Where(x => !categDict.ContainsKey(x)).ToList();
            if (notFoundCategNames.Any())
                return Ok(new { success = false, errors = new List<string>() { $"Đơn vị tính không tồn tại: {string.Join(", ", notFoundCategNames)}" } });

            var uoms = new List<UoM>();
            foreach (var item in data)
            {
                var categ = categDict[item.CategName];
                var uom = new UoM()
                {
                    Name = item.Name,
                    CategoryId = categ.Id,
                    UOMType = typeDict[item.Type],
                    Factor = (double)item.Factor,
                };

                if (uom.UOMType == "bigger")
                    uom.Factor = 1 / uom.Factor;

                uoms.Add(uom);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _uoMService.CreateAsync(uoms);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Lưu thất bại: {e.Message}" } });
            }

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\uom.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var exceptIds = await irModelObj.SearchQuery(x => x.Module == "product" && x.Model == "uom").Select(x => x.ResId).ToListAsync();
            var listIrModelData = await irModelObj.SearchQuery(x => x.Module == "product" && x.Model == "uom.category").ToListAsync();// các cate mặc định, 
            var entities = await _uoMService.SearchQuery(x => !exceptIds.Any(z => x.Id.ToString() == z) && x.DateCreated <= dateToData).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<UomXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<UomXmlSampleDataRecord>(entity);
                item.Id = $@"sample.uom_{entities.IndexOf(entity) + 1}";
                var irmodelData = listIrModelData.FirstOrDefault(x => x.ResId == item.CategoryId.ToString());
                item.CategoryId = irmodelData?.Module + "." + irmodelData?.Name;
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "uom",
                    ResId = entity.Id.ToString(),
                    Name = $"uom_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}