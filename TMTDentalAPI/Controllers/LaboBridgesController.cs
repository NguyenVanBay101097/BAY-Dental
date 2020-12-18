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
        public async Task<IActionResult> Get([FromQuery] LaboBridgesPaged val)
        {
            var result = await _LaboBridgeService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _LaboBridgeService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LaboBridgeSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _LaboBridgeService.CreateItem(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, LaboBridgeSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _LaboBridgeService.UpdateItem(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var product = await _LaboBridgeService.GetByIdAsync(id);
            if (product == null) return NotFound();

            await _LaboBridgeService.DeleteAsync(product);
            return NoContent();
        }

        [HttpPost("ImportExcel")]
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
    }
}
