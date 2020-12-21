using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaboFinishLinesController : BaseApiController
    {
        private readonly ILaboFinishLineService _laboFinishLineService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public LaboFinishLinesController(ILaboFinishLineService laboFinishLineService, IMapper mapper, IUnitOfWorkAsync unitOfWorkAsync)
        {
            _mapper = mapper;
            _laboFinishLineService = laboFinishLineService;
            _unitOfWork = unitOfWorkAsync;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.LaboFinishLine.Read")]
        public async Task<IActionResult> Get([FromQuery] LaboFinishLinesPaged val)
        {
            var result = await _laboFinishLineService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.LaboFinishLine.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _laboFinishLineService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.LaboFinishLine.Create")]
        public async Task<IActionResult> Create(LaboFinishLineSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _laboFinishLineService.CreateItem(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.LaboFinishLine.Update")]
        public async Task<IActionResult> Update(Guid id, LaboFinishLineSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _laboFinishLineService.UpdateItem(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.LaboFinishLine.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var product = await _laboFinishLineService.GetByIdAsync(id);
            if (product == null) return NotFound();

             await _laboFinishLineService.DeleteAsync(product);
            return NoContent();
        }

        [HttpPost("ImportExcel")]
        [CheckAccess(Actions = "Catalog.LaboFinishLine.Create")]
        public async Task<IActionResult> ImportExcel(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<LaboFinishLineImportExcelRow>();
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
                            errs.Add("Tên Đường hoàn tất Labo là bắt buộc");

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }

                        var item = new LaboFinishLineImportExcelRow
                        {
                            Name = name,
                        };
                        data.Add(item);
                    }
                }
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var vals = new List<LaboFinishLine>();
            foreach (var item in data)
            {
                var pd = new LaboFinishLine() { 
                Name = item.Name
                };
                vals.Add(pd);
            }

            await _laboFinishLineService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }
    }
}
