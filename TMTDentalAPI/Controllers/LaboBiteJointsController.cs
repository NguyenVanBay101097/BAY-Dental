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
        public async Task<IActionResult> Get([FromQuery] LaboBiteJointsPaged val)
        {
            var result = await _LaboBiteJointService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _LaboBiteJointService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LaboBiteJointSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _LaboBiteJointService.CreateItem(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, LaboBiteJointSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _LaboBiteJointService.UpdateItem(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var product = await _LaboBiteJointService.GetByIdAsync(id);
            if (product == null) return NotFound();

            await _LaboBiteJointService.DeleteAsync(product);
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
            var data = new List<LaboBiteJointImportExcelRow>();
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

                        var item = new LaboBiteJointImportExcelRow
                        {
                            Name = name,
                        };
                        data.Add(item);
                    }
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
    }
}
