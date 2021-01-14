using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockReportsController : BaseApiController
    {
        private readonly IStockReportService _stockReportService;
        public StockReportsController(IStockReportService stockReportService)
        {
            _stockReportService = stockReportService;
        }

        [HttpPost("XuatNhapTonSummary")]
        [CheckAccess(Actions = "Report.Stock")]
        public async Task<IActionResult> XuatNhapTonSummary(StockReportXuatNhapTonSearch val)
        {
            var res = await _stockReportService.XuatNhapTonSummary(val);
            return Ok(res);
        }

        [HttpPost("XuatNhapTonDetail")]
        [CheckAccess(Actions = "Report.Stock")]
        public async Task<IActionResult> XuatNhapTonDetail(StockReportXuatNhapTonItem val)
        {
            var res = await _stockReportService.XuatNhapTonDetail(val);
            return Ok(res);
        }
        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Stock")]
        public async Task<IActionResult> ExportExcelFile(StockReportXuatNhapTonSearch val)
        {
            var stream = new MemoryStream();
            var products = await _stockReportService.XuatNhapTonSummary(val);
            var sheetName = "Nhập xuất tồn";
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Mã sản phẩm";
                worksheet.Cells[1, 2].Value = "Tên sản phẩm";
                worksheet.Cells[1, 3].Value = "Đơn vị tính";
                worksheet.Cells[1, 4].Value = "Tồn đầu kỳ";
                worksheet.Cells[1, 5].Value = "Nhập trong kỳ";
                worksheet.Cells[1, 6].Value = "Xuất trong kỳ";
                worksheet.Cells[1, 7].Value = "Tồn cuối kỳ";

                var row = 2;
                foreach (var item in products)
                {
                    worksheet.Cells[row, 1].Value = item.ProductCode;
                    worksheet.Cells[row, 2].Value = item.ProductName;
                    worksheet.Cells[row, 3].Value = item.ProductUomName;
                    worksheet.Cells[row, 4].Value = item.Begin;
                    worksheet.Cells[row, 5].Value = item.Import;
                    worksheet.Cells[row, 6].Value = item.Export;
                    worksheet.Cells[row, 7].Value = item.End;

                    row++;
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