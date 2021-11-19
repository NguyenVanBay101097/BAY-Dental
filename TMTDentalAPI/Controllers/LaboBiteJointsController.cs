using System;
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
    public class LaboBiteJointsController : BaseApiController
    {
        private readonly ILaboBiteJointService _LaboBiteJointService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public LaboBiteJointsController(ILaboBiteJointService LaboBiteJointService, IMapper mapper, IUnitOfWorkAsync unitOfWorkAsync)
        {
            _mapper = mapper;
            _LaboBiteJointService = LaboBiteJointService;
            _unitOfWork = unitOfWorkAsync;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.LaboBiteJoint.Read")]
        public async Task<IActionResult> Get([FromQuery] LaboBiteJointsPaged val)
        {
            var result = await _LaboBiteJointService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.LaboBiteJoint.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _LaboBiteJointService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.LaboBiteJoint.Create")]
        public async Task<IActionResult> Create(LaboBiteJointSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _LaboBiteJointService.CreateItem(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.LaboBiteJoint.Update")]
        public async Task<IActionResult> Update(Guid id, LaboBiteJointSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _LaboBiteJointService.UpdateItem(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.LaboBiteJoint.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var product = await _LaboBiteJointService.GetByIdAsync(id);
            if (product == null) return NotFound();

            await _LaboBiteJointService.DeleteAsync(product);
            return NoContent();
        }

        [HttpPost("ImportExcel")]
        [CheckAccess(Actions = "Catalog.LaboBiteJoint.Create")]
        public async Task<IActionResult> ImportExcel(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<LaboBiteJointImportExcelRow>();
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
                                errs.Add("Tên khớp cắn Labo là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }

                            var item = new LaboBiteJointImportExcelRow
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

            var vals = new List<LaboBiteJoint>();
            foreach (var item in data)
            {
                var pd = new LaboBiteJoint()
                {
                    Name = item.Name
                };
                vals.Add(pd);
            }

            await _LaboBiteJointService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.LaboBiteJoint.Read")]
        public async Task<IActionResult> Autocomplete(LaboBiteJointPageSimple val)
        {
            var res = await _LaboBiteJointService.Autocomplete(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\labo_bite_joint.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var entities = await _LaboBiteJointService.SearchQuery(x => x.DateCreated <= dateToData).ToListAsync(); ;//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<SimpleXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<SimpleXmlSampleDataRecord>(entity);
                item.Id = $@"sample.labo_bite_joint_{entities.IndexOf(entity) + 1}";
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "labo.bite.joint",
                    ResId = entity.Id.ToString(),
                    Name = $"labo_bite_joint_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}
