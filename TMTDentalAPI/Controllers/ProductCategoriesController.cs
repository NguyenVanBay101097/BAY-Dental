﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using ApplicationCore.Models;
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
    public class ProductCategoriesController : BaseApiController
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IIRModelDataService _iRModelDataService;

        public ProductCategoriesController(IProductCategoryService productCategoryService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IIRModelAccessService modelAccessService, IIRModelDataService iRModelDataService)
        {
            _productCategoryService = productCategoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _modelAccessService = modelAccessService;
            _iRModelDataService = iRModelDataService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.ProductCategory.Read")]
        public async Task<IActionResult> Get([FromQuery] ProductCategoryPaged val)
        {
            var res = await _productCategoryService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.ProductCategory.Read")]
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
        [CheckAccess(Actions = "Catalog.ProductCategory.Create")]
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
        [CheckAccess(Actions = "Catalog.ProductCategory.Update")]
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
        [CheckAccess(Actions = "Catalog.ProductCategory.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _productCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _productCategoryService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        [CheckAccess(Actions = "Catalog.ProductCategory.Read")]
        public async Task<IActionResult> Autocomplete(ProductCategoryPaged val)
        {
            var res = await _productCategoryService.GetAutocompleteAsync(val);
            return Ok(res);
        }
        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.ProductCategory.Create")]
        public async Task<IActionResult> ImportExcel(ProductCategoryImportExcelBaseViewModel val)
        {
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
                pd.Type = val.Type;
                vals.Add(pd);
            }

            await _productCategoryService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_category.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var entities = await _productCategoryService.SearchQuery(x => x.Type != null && x.DateCreated <= dateToData).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<ProductCategoryXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<ProductCategoryXmlSampleDataRecord>(entity);
                item.Id = $@"sample.product_category_{entities.IndexOf(entity) + 1}";
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "product.category",
                    ResId = entity.Id.ToString(),
                    Name = $"product_category_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}