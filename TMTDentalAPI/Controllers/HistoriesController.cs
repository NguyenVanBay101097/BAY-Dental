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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHistoryService _service;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public HistoriesController(IMapper mapper, IHistoryService service, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _service = service;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]HistoryPaged val)
        {
            var result = await _service.GetPagedResultAsync(val);
            var paged = new PagedResult2<HistorySimple>(result.TotalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HistorySimple>>(result.Items)
            };
            return Ok(paged);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            var entity = _mapper.Map<HistorySimple>(result);
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HistorySimple val)
        {
            if (val == null)
                return BadRequest();
            var dup = await _service.CheckDuplicate(Guid.Empty,val);
            if (dup)
                throw new Exception("Bệnh này đã tồn tại !");

            var history = _mapper.Map<History>(val);
            await _service.CreateAsync(history);
            val.Id = history.Id;

            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HistorySimple val)
        {
            var result = await _service.GetByIdAsync(id);
            if(result==null)
                return NotFound();

            var dup = await _service.CheckDuplicate(id,val);
            if (dup)
                throw new Exception("Bệnh này đã tồn tại !");

            result = _mapper.Map(val,result);
            await _service.UpdateAsync(result);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            await _service.DeleteAsync(result);

            return NoContent();
        }

        [HttpGet("GetHistoriesCheckbox")]
        public async Task<IActionResult> GetHistoriesCheckbox()
        {
            var val = new HistoryPaged();
            var result = await _service.GetResultNotLimitAsync(val);
            var entity = _mapper.Map<IEnumerable<HistorySimple>>(result);
            return Ok(entity);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ImportExcel(HistoryImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
           
            await _unitOfWork.BeginTransactionAsync();

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<History>();

            var errors = new List<string>();

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        //var active = Convert.ToString(worksheet.Cells[row, 2].Value);

                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên tiểu sử bệnh là bắt buộc");


                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }



                        var item = new History
                        {
                            Name = name,
                            Active = true,
                        };
                        data.Add(item);
                    }
                }
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var vals = new List<History>();
            foreach (var item in data)
            {
                var pd = new History();
                pd.Name = item.Name;
                vals.Add(pd);
            }

            await _service.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }
    }
}