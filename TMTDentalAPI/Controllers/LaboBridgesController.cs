﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
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
    public class LaboBridgesController : BaseApiController
    {
        private readonly ILaboBridgeService _LaboBridgeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public LaboBridgesController(ILaboBridgeService LaboBridgeService, IMapper mapper, IUnitOfWorkAsync unitOfWorkAsync)
        {
            _mapper = mapper;
            _LaboBridgeService = LaboBridgeService;
            _unitOfWork = unitOfWorkAsync;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.LaboBridge.Read")]
        public async Task<IActionResult> Get([FromQuery] LaboBridgesPaged val)
        {
            var result = await _LaboBridgeService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.LaboBridge.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _LaboBridgeService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.LaboBridge.Create")]
        public async Task<IActionResult> Create(LaboBridgeSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _LaboBridgeService.CreateItem(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.LaboBridge.Update")]
        public async Task<IActionResult> Update(Guid id, LaboBridgeSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _LaboBridgeService.UpdateItem(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.LaboBridge.Read")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var product = await _LaboBridgeService.GetByIdAsync(id);
            if (product == null) return NotFound();

            await _LaboBridgeService.DeleteAsync(product);
            return NoContent();
        }

        [HttpPost("ImportExcel")]
        [CheckAccess(Actions = "Catalog.LaboBridge.Create")]
        public async Task<IActionResult> ImportExcel(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<LaboBridgeImportExcelRow>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                try
                {
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var errs = new List<string>();
                            var name = Convert.ToString(worksheet.Cells[row, 1].Value);

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên Đường hoàn tất Labo là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }

                            var item = new LaboBridgeImportExcelRow
                            {
                                Name = name,
                            };
                            data.Add(item);
                        }
                    }
                }
                catch (Exception)
                {

                    throw new Exception("Dữ liệu file không đúng định dạng mẫu");
                }
            }
            if (errors.Any())
                return Ok(new { success = false, errors });

            var vals = new List<LaboBridge>();
            foreach (var item in data)
            {
                var pd = new LaboBridge()
                {
                    Name = item.Name
                };
                vals.Add(pd);
            }

            await _LaboBridgeService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.LaboBridge.Read")]
        public async Task<IActionResult> Autocomplete(LaboBridgePageSimple val)
        {
            var res = await _LaboBridgeService.Autocomplete(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\labo_bridge.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var entities = await _LaboBridgeService.SearchQuery(x => x.DateCreated <= dateToData).ToListAsync(); ;//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<SimpleXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<SimpleXmlSampleDataRecord>(entity);
                item.Id = $@"sample.labo_bridge_{entities.IndexOf(entity) + 1}";
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "labo.bridge",
                    ResId = entity.Id.ToString(),
                    Name = $"labo_bridge_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}
