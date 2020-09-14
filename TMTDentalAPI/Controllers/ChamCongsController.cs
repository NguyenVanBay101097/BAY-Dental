using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.OpenXmlFormats.Spreadsheet;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChamCongsController : BaseApiController
    {
        private readonly IChamCongService _chamCongService;
        private readonly IHrPayslipService _hrPayslipService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public ChamCongsController(IHrPayslipService hrPayslipService, IEmployeeService employeeService, IChamCongService chamCongService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _chamCongService = chamCongService;
            _mapper = mapper;
            _hrPayslipService = hrPayslipService;
            _unitOfWork = unitOfWork;
            _employeeService = employeeService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Salary.ChamCong.Read")]
        public async Task<IActionResult> GetPaged([FromQuery] ChamCongPaged val)
        {
            var res = await _chamCongService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Salary.ChamCong.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var chamcong = await _mapper.ProjectTo<ChamCongDisplay>(_chamCongService.SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (chamcong == null)
                return NotFound();

            return Ok(chamcong);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> ExcelImportCreate(PartnerImportExcelViewModel val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var result = await _chamCongService.ImportExcel(val);
            if (result.Success)
                _unitOfWork.Commit();

            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Salary.ChamCong.Read")]
        public async Task<IActionResult> GetLastChamCong([FromQuery] employeePaged val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();

            var cc = await _chamCongService.GetLastChamCong(val);
            var res = _mapper.Map<ChamCongDisplay>(cc);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Salary.ChamCong.Create")]
        public async Task<IActionResult> Create(ChamCongSave val)
        {
            var chamcong = _mapper.Map<ChamCong>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _chamCongService.CreateAsync(chamcong);
            _unitOfWork.Commit();

            var display = _mapper.Map<ChamCongDisplay>(chamcong);
            return Ok(display);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Salary.ChamCong.Update")]
        public async Task<IActionResult> Update(Guid id, ChamCongSave val)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (chamcong == null)
                return NotFound();

            chamcong = _mapper.Map(val, chamcong);
            await _unitOfWork.BeginTransactionAsync();
            await _chamCongService.UpdateAsync(chamcong);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Salary.ChamCong.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (chamcong == null)
                return NotFound();

            await _chamCongService.DeleteAsync(chamcong);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(ChamCongDefaultGetPost val)
        {
            var res = await _chamCongService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.ChamCong.Create")]
        public async Task<IActionResult> TimeKeepingForAll(TimeKeepingForAll val)
        {
            _unitOfWork.BeginTransaction();
            await _chamCongService.TimeKeepingForAll(val);
            _unitOfWork.Commit();
            return Ok();
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
