using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : BaseApiController
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRModelAccessService _modelAccessService;

        public ProductCategoriesController(IProductCategoryService productCategoryService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IIRModelAccessService modelAccessService)
        {
            _productCategoryService = productCategoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _modelAccessService = modelAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductCategoryPaged val)
        {
            var res = await _productCategoryService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _productCategoryService.GetCategoryForDisplay(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductCategoryDisplay>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var category = _mapper.Map<ProductCategory>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _productCategoryService.CreateAsync(category);
            _unitOfWork.Commit();

            val.Id = category.Id;
            val.CompleteName = category.CompleteName;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductCategoryDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var category = await _productCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _unitOfWork.BeginTransactionAsync();
            await _productCategoryService.Write(category);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _productCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _productCategoryService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(ProductCategoryPaged val)
        {
            var res = await _productCategoryService.GetAutocompleteAsync(val);
            return Ok(res);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> ImportCategoryService(ProductCategoryImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductCategoryServiceImportExcelRow>();         
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);                       
                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên nhóm dịch vụ là bắt buộc");

                        var categ = await _productCategoryService.SearchQuery(x => x.Name == name && x.Type == "service").FirstOrDefaultAsync();
                        if (categ != null)                       
                            errors.Add($"Dòng {row}: {categ.Name} đã tồn tại");
                        
                           

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }

                        

                        var item = new ProductCategoryServiceImportExcelRow
                        {
                            Name = name
                           
                        };
                        data.Add(item);
                    }
                }
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var vals = new List<ProductCategory>();          
            foreach (var item in data)
            {
                var pd = new ProductCategory();              
                pd.Name = item.Name;
                pd.Type = "service";
                vals.Add(pd);
               
            }

            await _productCategoryService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ImportCategoryMedicine(ProductCategoryImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductCategoryMedicineImportExcelRow>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên nhóm thuốc là bắt buộc");

                        var categ = await _productCategoryService.SearchQuery(x => x.Name == name && x.Type == "medicine").FirstOrDefaultAsync();
                        if (categ != null)
                            errors.Add($"Dòng {row}: {categ.Name} đã tồn tại");

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }



                        var item = new ProductCategoryMedicineImportExcelRow
                        {
                            Name = name

                        };
                        data.Add(item);
                    }
                }
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var vals = new List<ProductCategory>();
            foreach (var item in data)
            {
                var pd = new ProductCategory();
                pd.Name = item.Name;
                pd.Type = "medicine";
                vals.Add(pd);

            }

            await _productCategoryService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ImportCategoryProduct(ProductCategoryImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductCategoryImportExcel>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên nhóm vật tư là bắt buộc");

                        var categ = await _productCategoryService.SearchQuery(x => x.Name == name && x.Type == "product").FirstOrDefaultAsync();
                        if (categ != null)
                            errors.Add($"Dòng {row}: {categ.Name} đã tồn tại");

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }



                        var item = new ProductCategoryImportExcel
                        {
                            Name = name


                        };
                        data.Add(item);
                    }
                }
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var vals = new List<ProductCategory>();
            foreach (var item in data)
            {
                var pd = new ProductCategory();
                pd.Name = item.Name;
                pd.Type = "product";
                vals.Add(pd);

            }

            await _productCategoryService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }
    }
}