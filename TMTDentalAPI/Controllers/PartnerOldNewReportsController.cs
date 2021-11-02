using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Utilities;
using DinkToPdf;
using DinkToPdf.Contracts;
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
    public class PartnerOldNewReportsController : ControllerBase
    {
        private IPartnerOldNewReportService _partnerOldNewReportService;
        private readonly IViewRenderService _viewRenderService;
        private IConverter _converter;
        private IExportExcelService _exportExcelService;
        public PartnerOldNewReportsController(IPartnerOldNewReportService partnerOldNewReportService, IViewRenderService viewRenderService,
            IExportExcelService exportExcelService,
            IConverter converter)
        {
            _partnerOldNewReportService = partnerOldNewReportService;
            _viewRenderService = viewRenderService;
            _converter = converter;
            _exportExcelService = exportExcelService;
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.PartnerOldNew")]
        public async Task<IActionResult> GetPartnerOldNewReport(PartnerOldNewReportSearch val)
        {
            var res = await _partnerOldNewReportService.GetPartnerOldNewReport(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.PartnerOldNew")]
        public async Task<IActionResult> GetSumaryPartnerOldNewReport(PartnerOldNewReportSearch val)
        {
            var res = await _partnerOldNewReportService.GetSumaryPartnerOldNewReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReport([FromQuery] PartnerOldNewReportReq val)
        {
            var res = await _partnerOldNewReportService.GetReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SumReport([FromQuery] PartnerOldNewReportReq val)
        {
            var res = await _partnerOldNewReportService.SumReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SumReVenue([FromQuery] PartnerOldNewReportReq val)
        {
            var res = await _partnerOldNewReportService.SumReVenue(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReportByWard([FromQuery] PartnerOldNewReportByWardReq val)
        {
            var res = await _partnerOldNewReportService.ReportByWard(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReportPdf([FromQuery] PartnerOldNewReportReq val)
        {
            var data = await _partnerOldNewReportService.GetReportPrint(val);
            var html = _viewRenderService.Render("PartnerOldNewReport/GetReportPdf", data);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css", "print.css") },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo dịch vụ", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "baocaokhachhang.pdf");
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetSaleOrderPaged([FromQuery] GetSaleOrderPagedReq val)
        {
            var res = await _partnerOldNewReportService.GetSaleOrderPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReportExcel([FromQuery] PartnerOldNewReportReq val)
        {
            val.Limit = int.MaxValue;
            var stream = new MemoryStream();
            var data = await _partnerOldNewReportService.GetReportExcel(val);
            byte[] fileContent;
            var dateToDate = (data.DateFrom.HasValue ? "Từ ngày " + data.DateFrom.Value.ToString("dd/MM/yyyy") : "") +
                                               (data.DateTo.HasValue ? " đến ngày " + data.DateTo.Value.ToString("dd/MM/yyyy") : "");
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("ThongKeKhachHangDieuTri");
                worksheet.Cells["A1:H1"].Value = "THỐNG KÊ KHÁCH HÀNG ĐIỀU TRỊ";
                worksheet.Cells["A1:H1"].Style.Font.Size = 14;
                worksheet.Cells["A1:H1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A2:H2"].Value = dateToDate;
                worksheet.Cells["A2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H2"].Merge = true;
                worksheet.Cells["A3:H3"].Value = "";
                
                worksheet.Cells["A4"].Value = "Khách hàng";
                worksheet.Cells["B4"].Value = "Tuổi";
                worksheet.Cells["B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["C4"].Value = "Giới tính";
                worksheet.Cells["D4"].Value = "Địa chỉ";
                worksheet.Cells["E4"].Value = "Doanh thu";
                worksheet.Cells["E4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["F4"].Value = "Nguồn";
                worksheet.Cells["G4"].Value = "Nhãn";
                worksheet.Cells["H4"].Value = "Tình trạng điều trị";
                worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                worksheet.Cells["A4:H4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:H4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.White);

                var row = 5;
                foreach (var item in data.Data)
                {
                    //add parent data
                    worksheet.Cells[row, 1].Value = item.Name;
                    worksheet.Cells[row, 2].Value = item.Age;
                    worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[row, 3].Value = item.Gender == "male"? "Nam" : (item.Gender == "female" ? "Nữ" : "Khác");
                    worksheet.Cells[row, 4].Value = item.Address;
                    worksheet.Cells[row, 5].Value = item.Revenue;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 6].Value = item.SourceName;
                    worksheet.Cells[row, 7].Value = string.Join("; ",item.Categories.Select(x=> x.Name));
                    worksheet.Cells[row, 8].Value = item.OrderState == "sale" ? "Đang điều trị" : (item.OrderState == "done" ? "Hoàn thành" : "Chưa phát sinh") ;

                    row++;
                    //add child header
                    worksheet.Cells[row, 2, row, 3].Value = "Ngày tạo";
                    worksheet.Cells[row, 2, row, 3].Merge = true;
                    worksheet.Cells[row, 4].Value = "Phiếu điều trị";
                    worksheet.Cells[row, 5].Value = "Tổng tiền";
                    worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[row, 6].Value = "Thanh toán";
                    worksheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[row, 7].Value = "Còn lại";
                    worksheet.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[row, 8].Value = "Trạng thái";
                    worksheet.Cells[row, 2, row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
                    worksheet.Cells[row, 1, row, worksheet.Dimension.Columns].Style.Font.Bold = true;
                    var rowEnd = row + item.Lines.Count();
                    worksheet.Cells[row, 1, rowEnd, 1].Merge = true;
                    row++;
                    //add child data
                    foreach (var line in item.Lines)
                    {
                        worksheet.Cells[row, 2, row, 3].Value = line.DateOrder;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/mm/yyyy";
                        worksheet.Cells[row, 2, row, 3].Merge = true;
                        worksheet.Cells[row, 2, row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 4].Value = line.Name;
                        worksheet.Cells[row, 5].Value = line.AmountTotal;
                        worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 6].Value = line.TotalPaid;
                        worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 7].Value = line.Residual;
                        worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 8].Value = line.State == "sale" ? "Đang điều trị" : (line.State == "done" ? "Hoàn thành" : "Nháp");
                        row++;
                    }
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ReportByIsNew([FromQuery] PartnerOldNewReportByIsNewReq val)
        {
            var res = await _partnerOldNewReportService.ReportByIsNew(val);
            return Ok(res);
        }
    }
}
