using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Services;
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
    public class CashBooksController : ControllerBase
    {
        private readonly ICashBookService _fundBookService;
        public CashBooksController(ICashBookService fundBookService)
        {
            _fundBookService = fundBookService;
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> GetMoney(CashBookSearch val)
        {
            var res = await _fundBookService.GetMoney(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> GetSumary(CashBookSearch val)
        {
            var res = await _fundBookService.GetSumary(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetTotalReport(CashBookSearch val)
        {
            var res = await _fundBookService.GetTotalReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> ExportExcelFile([FromQuery] CashBookSearch val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var services = await _fundBookService.GetExportExcel(val);
            var sheetName = "Tổng sổ quỹ";
            if (val.ResultSelection == "cash")
            {
                sheetName = "Sổ quỹ tiền mặt";
            }
            else if (val.ResultSelection == "bank")
            {
                sheetName = "Sổ quỹ ngân hàng";
            }

            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Số phiếu";
                worksheet.Cells[1, 3].Value = "Loại thu chi";
                worksheet.Cells[1, 4].Value = "Tiền chi";
                worksheet.Cells[1, 5].Value = "Tiền thu";
                worksheet.Cells[1, 6].Value = "Người nhận/ nộp";
                for (int row = 2; row < services.Count() + 2; row++)
                {
                    var item = services.ToList()[row - 2];

                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.Ref;
                    worksheet.Cells[row, 4].Value = item.Credit;
                    worksheet.Cells[row, 5].Value = item.Debit;
                    worksheet.Cells[row, 6].Value = item.PartnerName;
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }
    }
}
