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
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.OpenXmlFormats.Spreadsheet;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChamCongsController : BaseApiController
    {
        private readonly IChamCongService _chamCongService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public ChamCongsController(IChamCongService chamCongService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _chamCongService = chamCongService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] employeePaged val)
        {
            var res = await _chamCongService.GetAll(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).Include(x => x.Employee).FirstOrDefaultAsync();
            if (chamcong == null)
                return NotFound();
            var res = _mapper.Map<ChamCongDisplay>(chamcong);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChamCongSave chamCongSave)
        {
            var chamcong = _mapper.Map<ChamCong>(chamCongSave);
            chamcong.CompanyId = _chamCongService.GetCurrentCompanyId();
            await _chamCongService.CreateAsync(chamcong);
            return Ok(_mapper.Map<ChamCongDisplay>(chamcong));
        }

        //[HttpPost("CreateList")]
        //public async Task<IActionResult> CreateList(IEnumerable<ChamCongSave> chamCongSaves)
        //{
        //    await _chamCongService.CreateListChamcongs(_mapper.Map<IEnumerable<ChamCong>>(chamCongSaves));
        //    return Ok();
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ChamCongSave chamCongSave)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (chamcong == null)
            {
                return NotFound();
            }
            chamcong = _mapper.Map(chamCongSave, chamcong);
            chamcong.CompanyId = _chamCongService.GetCurrentCompanyId();
            await _chamCongService.UpdateAsync(chamcong);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (chamcong == null)
            {
                return NotFound();
            }
            await _chamCongService.DeleteAsync(chamcong);
            return NoContent();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByEmployee(Guid id, DateTime date)
        {
            var res = await _chamCongService.GetByEmployeeId(id, date);
            return Ok(res);
        }

        //[HttpPost("[action]")]
        //public async Task<IActionResult> ExportExcelFile(employeePaged val)
        //{
        //    var stream = new MemoryStream();
        //    var data = await _chamCongService.GetByEmployeePaged(val);
        //    byte[] fileContent;
        //    int dateStart = val.From.HasValue ? val.From.Value.Day : 1;
        //    int dateEnd = val.To.HasValue ? val.To.Value.Day : 1;
        //    using (var package = new ExcelPackage(stream))
        //    {


        //        var worksheet = package.Workbook.Worksheets.Add("Sheet1");


        //        worksheet.Cells[2, 1].Value = "Tên nhân viên";
        //        worksheet.Cells[2, 2].Value = "Chức vụ";

        //        for (int i = dateStart + 2; i <= dateEnd + 2; i++)
        //        {
        //            worksheet.Cells[2, i].Value = i - 2;
        //        }

        //        worksheet.Cells["A1:K1"].Style.Font.Bold = true;

        //        var row = 3;
        //        foreach (var item in data.Items)
        //        {
        //            worksheet.Cells[row, 1].Value = item.Name + $" ({item.Ref})";
        //            worksheet.Cells[row, 2].Value = item.IsDoctor.Value == true ? "Bác sĩ" : (item.IsAssistant.Value == true ? "Y tá" : "Chưa có chức vụ");
        //            for (int i = dateStart; i <= dateEnd; i++)
        //            {
        //                foreach (var cc in item.ChamCongs)
        //                {

        //                    if ((cc.TimeIn.HasValue ? cc.TimeIn.Value.Day : (cc.TimeOut.HasValue ? cc.TimeOut.Value.Day : 0)) == i)
        //                    {
        //                        worksheet.Cells[row, i + 2].Value = "vào: " + (cc.TimeIn.HasValue ? cc.TimeIn.Value.ToString("HH:mm") : "chưa chấm") + "\r\n" + " ra: " + (cc.TimeOut.HasValue ? cc.TimeOut.Value.ToString("HH:mm") : "chưa chấm");
        //                    }
        //                }
        //            }

        //            row++;
        //        }

        //        worksheet.Column(4).Style.Numberformat.Format = "@";
        //        worksheet.Column(5).Style.Numberformat.Format = "@";

        //        package.Save();

        //        fileContent = stream.ToArray();
        //    }

        //    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    stream.Position = 0;

        //    return new FileContentResult(fileContent, mimeType);
        //}
    }
}
