using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionSettlementsController : BaseApiController
    {
        private readonly ICommissionSettlementService _commissionSettlementService;
        private readonly IExportExcelService _exportExcelService;
        public CommissionSettlementsController(ICommissionSettlementService commissionSettlementService, IExportExcelService exportExcelService)
        {
            _commissionSettlementService = commissionSettlementService;
            _exportExcelService = exportExcelService;
        }

        [HttpPost("[action]")]      
        public async Task<IActionResult> GetReport(CommissionSettlementFilterReport val)
        {
            var result = await _commissionSettlementService.GetReport(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCommissionSettlementReport(CommissionSettlementOverviewFilter val)
        {
            var result = await _commissionSettlementService.GetCommissionSettlements(dateFrom: val.DateFrom, dateTo: val.DateTo,classify: val.Classify , groupBy: val.GroupBy);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReportPaged([FromQuery] CommissionSettlementFilterReport val)
        {
            var result = await _commissionSettlementService.GetReportPaged(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReportDetail(CommissionSettlementFilterReport val)
        {
            var result = await _commissionSettlementService.GetReportDetail(val);
            return Ok(result);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetSumReport(CommissionSettlementFilterReport val)
        {
            var result = await _commissionSettlementService.GetSumReport(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSumAmountTotalReport(CommissionSettlementFilterReport val)
        {
            var result = await _commissionSettlementService.GetSumAmountTotalReport(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcel([FromQuery] CommissionSettlementFilterReport val)
        {

            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            var data = await _commissionSettlementService.GetReportPaged(val);
            decimal sum = data.Sum(x => x.Amount.Value);
            byte[] fileContent;
            var sheetName = "Hoa hồng nhân viên";


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Nhân viên";
                worksheet.Cells[1, 2].Value = "Loại hoa hồng";
                worksheet.Cells[1, 3].Value = "Tiền hoa hồng";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.EmployeeName;
                    worksheet.Cells[row, 2].Value = _commissionSettlementService.CommissionType(item.CommissionType);
                    worksheet.Cells[row, 3].Value = item.Amount;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,###";

                    row++;
                }

                worksheet.Cells[row, 1].Value = "Tổng";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = sum;
                worksheet.Cells[row, 2, row, 3].Merge = true;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,###";
                worksheet.Cells[row, 2].Style.Font.Bold = true;

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);

        }



        [HttpGet("[action]")]
        public async Task<IActionResult> DetailExportExcel([FromQuery] CommissionSettlementFilterReport val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            var data = await _commissionSettlementService.GetReportDetail(val);
            byte[] fileContent;
            var sheetName = val.GroupBy == "employee" ? "Chi tiết hoa hồng nhân viên" : "CTHoaHongNguoiGioiThieu";


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Ngày thanh toán";
                worksheet.Cells[1, 2].Value = (val.GroupBy == "employee" ? "Nhân viên" : "Người giới thiệu");
                worksheet.Cells[1, 3].Value = (val.GroupBy == "employee" ? "Loại hoa hồng" : "Phân loại");
                worksheet.Cells[1, 4].Value = "Số phiếu";
                worksheet.Cells[1, 5].Value = "Khách hàng";
                worksheet.Cells[1, 6].Value = "Dịch vụ";
                worksheet.Cells[1, 7].Value = "Đơn vị tính";
                worksheet.Cells[1, 8].Value = "Lợi nhuận tính hoa hồng";
                worksheet.Cells[1, 9].Value = "% Hoa hồng";
                worksheet.Cells[1, 10].Value = "Tiền hoa hồng";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;
                worksheet.Cells["A1:P1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:P1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.White);

                var row = 2;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value =  item.Name;
                    worksheet.Cells[row, 3].Value = (val.GroupBy == "employee" ? _commissionSettlementService.CommissionType(item.CommissionType) : _commissionSettlementService.Classify(item.Classify)); 
                    worksheet.Cells[row, 4].Value = item.InvoiceOrigin;
                    worksheet.Cells[row, 5].Value = item.PartnerName;
                    worksheet.Cells[row, 6].Value = item.ProductName;
                    worksheet.Cells[row, 7].Value = item.UOMName;
                    worksheet.Cells[row, 8].Value = item.BaseAmount;
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "#,###";
                    worksheet.Cells[row, 9].Value = item.Percentage;
                    worksheet.Cells[row, 9].Style.Numberformat.Format = "#0\\%";
                    worksheet.Cells[row, 10].Value = item.Amount;
                    worksheet.Cells[row, 10].Style.Numberformat.Format = "#,###";

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

        [HttpGet("[action]")]
        public async Task<IActionResult> ItemDetailExportExcel([FromQuery] CommissionSettlementFilterReport val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            var data = await _commissionSettlementService.GetReportDetail(val);
            byte[] fileContent;
            var sheetName = "HoaHongNguoiGioiThieu";


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells["A1:H1"].Value = data.Items.ToArray()[0].Name;
                worksheet.Cells["A1:H1"].Style.Font.Size = 14;
                //worksheet.Cells["A1:H1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;

                worksheet.Cells[3, 1].Value = "Ngày thanh toán";
                worksheet.Cells[3, 2].Value = "Số phiếu";
                worksheet.Cells[3, 3].Value = "Khách hàng";
                worksheet.Cells[3, 4].Value = "Dịch vụ";
                worksheet.Cells[3, 5].Value = "Đơn vị tính";
                worksheet.Cells[3, 6].Value = "Lợi nhuận tính hoa hồng";
                worksheet.Cells[3, 7].Value = "% Hoa hồng";
                worksheet.Cells[3, 8].Value = "Tiền hoa hồng";
                worksheet.Cells[3, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                worksheet.Cells["A3:H3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A3:H3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.White);

                var row = 4;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";             
                    worksheet.Cells[row, 2].Value = item.InvoiceOrigin;
                    worksheet.Cells[row, 3].Value = item.PartnerName;
                    worksheet.Cells[row, 4].Value = item.ProductName;
                    worksheet.Cells[row, 5].Value = item.UOMName;
                    worksheet.Cells[row, 6].Value = item.BaseAmount;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "#,###";
                    worksheet.Cells[row, 7].Value = item.Percentage;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "#0\\%";
                    worksheet.Cells[row, 8].Value = item.Amount;
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "#,###";
                    worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);



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
