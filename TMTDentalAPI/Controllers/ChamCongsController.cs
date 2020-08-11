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
        public async Task<IActionResult> Get([FromQuery] employeePaged val)
        {
            var res = await _chamCongService.GetByEmployeePaged(val);
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
            if (chamCongSave.Status == "NP")
            {
                chamCongSave.HourWorked = await _chamCongService.GetStandardWorkHour();
            }
            var chamcong = _mapper.Map<ChamCong>(chamCongSave);
            chamcong.CompanyId = _chamCongService.GetCurrentCompanyId();
            await _chamCongService.CreateAsync(chamcong);
            return Ok(_mapper.Map<ChamCongDisplay>(chamcong));
        }

        [HttpPost("CreateList")]
        public async Task<IActionResult> CreateList(IEnumerable<ChamCongSave> chamCongSaves)
        {
            await _chamCongService.CreateListChamcongs(_mapper.Map<IEnumerable<ChamCong>>(chamCongSaves));
            return Ok();
        }

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
        //public async Task<IActionResult> ExportExcelFile(PartnerPaged val)
        //{
            //var stream = new MemoryStream();
            //var data = await _chamCongService.GetExcel(val);
            //byte[] fileContent;

            //var gender_dict = new Dictionary<string, string>()
            //{
            //    { "male", "Nam" },
            //    { "female", "Nữ" },
            //    { "other", "Khác" }
            //};

            //using (var package = new ExcelPackage(stream))
            //{
            //    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            //    worksheet.Cells[1, 1].Value = "Tên KH";
            //    worksheet.Cells[1, 2].Value = "Mã KH";
            //    worksheet.Cells[1, 3].Value = "Ngày tạo";
            //    worksheet.Cells[1, 4].Value = "Giới tính";
            //    worksheet.Cells[1, 5].Value = "Ngày sinh";
            //    worksheet.Cells[1, 6].Value = "SĐT";
            //    worksheet.Cells[1, 7].Value = "Địa chỉ";
            //    worksheet.Cells[1, 8].Value = "Tiểu sử bệnh";
            //    worksheet.Cells[1, 9].Value = "Nghề nghiệp";
            //    worksheet.Cells[1, 10].Value = "Email";
            //    worksheet.Cells[1, 11].Value = "Ghi chú";

            //    worksheet.Cells["A1:K1"].Style.Font.Bold = true;

            //    var row = 2;
            //    foreach (var item in data)
            //    {
            //        worksheet.Cells[row, 1].Value = item.Name;
            //        worksheet.Cells[row, 2].Value = item.Ref;
            //        worksheet.Cells[row, 3].Value = item.Date;
            //        worksheet.Cells[row, 3].Style.Numberformat.Format = "d/m/yyyy";
            //        worksheet.Cells[row, 4].Value = !string.IsNullOrEmpty(item.Gender) && gender_dict.ContainsKey(item.Gender) ? gender_dict[item.Gender] : "Nam";
            //        worksheet.Cells[row, 5].Value = item.DateOfBirth;
            //        worksheet.Cells[row, 6].Value = item.Phone;
            //        worksheet.Cells[row, 7].Value = item.Address;
            //        worksheet.Cells[row, 8].Value = string.Join(",", item.MedicalHistories);
            //        worksheet.Cells[row, 9].Value = item.Job;
            //        worksheet.Cells[row, 10].Value = item.Email;
            //        worksheet.Cells[row, 11].Value = item.Note;

            //        row++;
            //    }

            //    worksheet.Column(4).Style.Numberformat.Format = "@";
            //    worksheet.Column(5).Style.Numberformat.Format = "@";

            //    package.Save();

            //    fileContent = stream.ToArray();
            //}

            //string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //stream.Position = 0;

            //return new FileContentResult(fileContent, mimeType);
        //}
    }
}
