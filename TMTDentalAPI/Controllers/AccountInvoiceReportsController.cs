using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    //Báo cáo doanh thu controller
    [Route("api/[controller]")]
    [ApiController]
    public class AccountInvoiceReportsController : BaseApiController
    {
        private readonly IAccountInvoiceReportService _invoiceReportService;
        private readonly IProductService _productService;
        private readonly IViewRenderService _viewRenderService;
        private IConverter _converter;
        public AccountInvoiceReportsController(IAccountInvoiceReportService invoiceReportService, IProductService productService,
            IViewRenderService viewRenderService, IConverter converter)
        {
            _productService = productService;
            _invoiceReportService = invoiceReportService;
            _viewRenderService = viewRenderService;
            _converter = converter;
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueTimeReportPaged([FromQuery] RevenueTimeReportPar val)
        {
            //báo cáo doanh thu theo ngày
            var res = await _invoiceReportService.GetRevenueTimeReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueTimeByMonth([FromQuery] RevenueTimeReportPar val)
        {
            //báo cáo doanh thu theo tháng
            var res = await _invoiceReportService.GetRevenueTimeByMonth(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueServiceReportPaged([FromQuery] RevenueServiceReportPar val)
        {
            var res = await _invoiceReportService.GetRevenueServiceReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueEmployeeReportPaged([FromQuery] RevenueEmployeeReportPar val)
        {
            var res = await _invoiceReportService.GetRevenueEmployeeReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueReportDetailPaged([FromQuery] RevenueReportDetailPaged val)
        {
            var res = await _invoiceReportService.GetRevenueReportDetailPaged(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueReport(RevenueReportFilter val)
        {
            var res = await _invoiceReportService.GetRevenueReport(val);
            return Ok(res);
        }

        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetRevenueReportDetailPrint([FromQuery] RevenueReportDetailPaged val)
        //{
        //    var res = await _invoiceReportService.GetRevenueReportDetailPaged(val);

        //    if (res.Items == null) return NotFound();
        //    var html = _viewRenderService.Render("AccountInvoiceReport/PrintRevenueReportDetail", res.Items);

        //    return Ok(new PrintData() { html = html });
        //}

        [HttpGet("[action]")]
        public async Task<IActionResult> SumRevenueReport([FromQuery]SumRevenueReportPar val)
        {
            var res = await _invoiceReportService.SumRevenueReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenuePartnerReportPaged([FromQuery] RevenuePartnerReportPar val)
        {
            var res = await _invoiceReportService.GetRevenuePartnerReport(val);
            return Ok(res);
        }     

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenuePartnerReportPdf([FromQuery] RevenuePartnerReportPar val)
        {
            var data = await _invoiceReportService.GetRevenuePartnerReportPrint(val);
            var html = _viewRenderService.Render("AccountInvoiceReport/RevenuePartnerReportPdf", data);

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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo doanh thu", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "baocaodoanhthu_theoKH.pdf");
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueTimeReportPdf([FromQuery] RevenueTimeReportPar val)
        {
            var data = await _invoiceReportService.GetRevenueTimeReportPrint(val);
            var html = _viewRenderService.Render("AccountInvoiceReport/RevenueTimeReportPdf", data);

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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo doanh thu", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "baocaodoanhthu_theoTG.pdf");
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueServiceReportPdf([FromQuery] RevenueServiceReportPar val)
        {
            var data = await _invoiceReportService.GetRevenueServiceReportPrint(val);
            var html = _viewRenderService.Render("AccountInvoiceReport/RevenueServiceReportPdf", data);

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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo doanh thu", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "baocaodoanhthu_theoDV.pdf");
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> GetRevenueEmployeeReportPdf([FromQuery] RevenueEmployeeReportPar val)
        {
            var data = await _invoiceReportService.GetRevenueEmployeeReportPrint(val);
            var html = _viewRenderService.Render("AccountInvoiceReport/RevenueEmployeeReportPdf", data);

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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo doanh thu", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "baocaodoanhthu_theoNV.pdf");
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> ExportRevenueTimeReportExcel([FromBody] RevenueTimeReportPar val)
        {
            var data = await _invoiceReportService.GetRevenueTimeReportExcel(val);
            var dateToDate = "";
            if (val.DateFrom.HasValue && val.DateTo.HasValue)
            {
                dateToDate = $"Từ ngày {val.DateFrom.Value.ToString("dd/MM/yyyy")} đến ngày {val.DateTo.Value.ToString("dd/MM/yyyy")}";
            }
            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoDoanhThu_TheoTG");

                worksheet.Cells["A1:G1"].Value = data.Title;
                worksheet.Cells["A1:G1"].Style.Font.Size = 14;
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:G1"].Merge = true;
                worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                worksheet.Cells["A2:G2"].Value = dateToDate;
                worksheet.Cells["A2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A3:G3"].Value = "";
                worksheet.Cells["A4:A4"].Value = data.ColumnTitle;
                worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                worksheet.Cells["A4:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:G4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["B4:G4"].Value = "Doanh thu";
                worksheet.Cells["B4:G4"].Merge = true;
                worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells["B4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["A4:G4"].Style.Font.Size = 14;

                var row = 5;
                foreach (var item in data.Data)
                {
                    worksheet.Cells[row, 1].Value = item.InvoiceDate;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2, row, 7].Value = item.PriceSubTotal;
                    worksheet.Cells[row, 2, row, 7].Style.Numberformat.Format = "#,###,###";
                    worksheet.Cells[row, 2, row, 7].Merge = true;
                    row++;
                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "Số phiếu";
                    worksheet.Cells[row, 3].Value = "Khách hàng";
                    worksheet.Cells[row, 4].Value = "Bác sĩ";
                    worksheet.Cells[row, 5].Value = "Phụ tá";
                    worksheet.Cells[row, 6].Value = "Dịch vụ/Thuốc";
                    worksheet.Cells[row, 7].Value = "Thanh toán";
                    worksheet.Cells[row, 2, row, 7].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 2, row, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
                    var rowEnd = row + item.Lines.Count();
                    worksheet.Cells[row, 1, rowEnd, 1].Merge = true;
                    row++;

                    foreach (var line in item.Lines)
                    {
                        worksheet.Cells[row, 1].Value = "";
                        worksheet.Cells[row, 2].Value = line.InvoiceOrigin;
                        worksheet.Cells[row, 3].Value = line.PartnerName;
                        worksheet.Cells[row, 4].Value = line.EmployeeName;
                        worksheet.Cells[row, 5].Value = line.AssistantName;
                        worksheet.Cells[row, 6].Value = line.ProductName;
                        worksheet.Cells[row, 7].Value = line.PriceSubTotal;
                        worksheet.Cells[row, 7].Style.Numberformat.Format = "#,###,###";
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
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> ExportRevenueServiceReportExcel([FromBody] RevenueServiceReportPar val)
        {
            var data = await _invoiceReportService.GetRevenueServiceReportExcel(val);
            var dateToDate = "";
            if (val.DateFrom.HasValue && val.DateTo.HasValue)
            {
                dateToDate = $"Từ ngày {val.DateFrom.Value.ToString("dd/MM/yyyy")} đến ngày {val.DateTo.Value.ToString("dd/MM/yyyy")}";
            }
            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoDoanhThu_TheoDV");

                worksheet.Cells["A1:H1"].Value = data.Title;
                worksheet.Cells["A1:H1"].Style.Font.Size = 14;
                worksheet.Cells["A1:H1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A2:H2"].Value = dateToDate;
                worksheet.Cells["A2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H2"].Merge = true;
                worksheet.Cells["A3:H3"].Value = "";
                worksheet.Cells["A4:A4"].Value = data.ColumnTitle;
                worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                worksheet.Cells["A4:H4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:H4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["B4:H4"].Value = "Doanh thu";
                worksheet.Cells["B4:H4"].Merge = true;
                worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells["B4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["A4:H4"].Style.Font.Size = 14;

                var row = 5;
                foreach (var item in data.Data)
                {
                    worksheet.Cells[row, 1].Value = !string.IsNullOrEmpty(item.ProductName) ? item.ProductName : "Không xác định";
                    
                    worksheet.Cells[row, 2, row, 8].Value = item.PriceSubTotal;
                    worksheet.Cells[row, 2, row, 8].Style.Numberformat.Format = "#,###,###";
                    worksheet.Cells[row, 2, row, 8].Merge = true;
                    row++;

                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "Ngày thanh toán";
                    worksheet.Cells[row, 3].Value = "Số phiếu";
                    worksheet.Cells[row, 4].Value = "Khách hàng";
                    worksheet.Cells[row, 5].Value = "Bác sĩ";
                    worksheet.Cells[row, 6].Value = "Phụ tá";
                    worksheet.Cells[row, 7].Value = "Dịch vụ/Thuốc";
                    worksheet.Cells[row, 8].Value = "Thanh toán";
                    worksheet.Cells[row, 2, row, 8].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
                    var rowEnd = row + item.Lines.Count();
                    worksheet.Cells[row, 1, rowEnd, 1].Merge = true;
                    row++;

                    foreach (var line in item.Lines)
                    {
                        worksheet.Cells[row, 1].Value = "";
                        worksheet.Cells[row, 2].Value = line.InvoiceDate;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/mm/yyyy";
                        worksheet.Cells[row, 3].Value = line.InvoiceOrigin;
                        worksheet.Cells[row, 4].Value = line.PartnerName;
                        worksheet.Cells[row, 5].Value = line.EmployeeName;
                        worksheet.Cells[row, 6].Value = line.AssistantName;
                        worksheet.Cells[row, 7].Value = line.ProductName;
                        worksheet.Cells[row, 8].Value = line.PriceSubTotal;
                        worksheet.Cells[row, 8].Style.Numberformat.Format = "#,###,###";
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
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> ExportRevenueEmployeeReportExcel([FromBody] RevenueEmployeeReportPar val)
        {
            var data = await _invoiceReportService.GetRevenueEmployeeReportExcel(val);
            var dateToDate = "";
            if (val.DateFrom.HasValue && val.DateTo.HasValue)
            {
                dateToDate = $"Từ ngày {val.DateFrom.Value.ToString("dd/MM/yyyy")} đến ngày {val.DateTo.Value.ToString("dd/MM/yyyy")}";
            }
            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoDoanhThu_TheoNV");

                worksheet.Cells["A1:H1"].Value = data.Title;
                worksheet.Cells["A1:H1"].Style.Font.Size = 14;
                worksheet.Cells["A1:H1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A2:H2"].Value = dateToDate;
                worksheet.Cells["A2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H2"].Merge = true;
                worksheet.Cells["A3:H3"].Value = "";
                worksheet.Cells["A4:A4"].Value = data.ColumnTitle;
                worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                worksheet.Cells["A4:H4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:H4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["B4:H4"].Value = "Doanh thu";
                worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells["B4:H4"].Merge = true;
                worksheet.Cells["B4:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["A4:H4"].Style.Font.Size = 14;

                var row = 5;
                foreach (var item in data.Data)
                {
                    worksheet.Cells[row, 1].Value = !string.IsNullOrEmpty(item.EmployeeName) ? item.EmployeeName : "Không xác định";

                    worksheet.Cells[row, 2, row, 8].Value = item.PriceSubTotal;
                    worksheet.Cells[row, 2, row, 8].Style.Numberformat.Format = "#,###,###";
                    worksheet.Cells[row, 2, row, 8].Merge = true;
                    row++;

                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "Ngày thanh toán";
                    worksheet.Cells[row, 3].Value = "Số phiếu";
                    worksheet.Cells[row, 4].Value = "Khách hàng";
                    worksheet.Cells[row, 5].Value = "Bác sĩ";
                    worksheet.Cells[row, 6].Value = "Phụ tá";
                    worksheet.Cells[row, 7].Value = "Dịch vụ/Thuốc";
                    worksheet.Cells[row, 8].Value = "Thanh toán";
                    worksheet.Cells[row, 2, row, 8].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
                    var rowEnd = row + item.Lines.Count();
                    worksheet.Cells[row, 1, rowEnd, 1].Merge = true;
                    row++;

                    foreach (var line in item.Lines)
                    {
                        worksheet.Cells[row, 1].Value = "";
                        worksheet.Cells[row, 2].Value = line.InvoiceDate;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/mm/yyyy";
                        worksheet.Cells[row, 3].Value = line.InvoiceOrigin;
                        worksheet.Cells[row, 4].Value = line.PartnerName;
                        worksheet.Cells[row, 5].Value = line.EmployeeName;
                        worksheet.Cells[row, 6].Value = line.AssistantName;
                        worksheet.Cells[row, 7].Value = line.ProductName;
                        worksheet.Cells[row, 8].Value = line.PriceSubTotal;
                        worksheet.Cells[row, 8].Style.Numberformat.Format = "#,###,###";
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
        [CheckAccess(Actions = "Report.Revenue")]
        public async Task<IActionResult> ExportRevenuePartnerReportExcel([FromBody] RevenuePartnerReportPar val)
        {
            var data = await _invoiceReportService.GetRevenuePartnerReportExcel(val);
            var dateToDate = "";
            if (val.DateFrom.HasValue && val.DateTo.HasValue)
            {
                dateToDate = $"Từ ngày {val.DateFrom.Value.ToString("dd/MM/yyyy")} đến ngày {val.DateTo.Value.ToString("dd/MM/yyyy")}";
            }
            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoDoanhThu_TheoKH");

                worksheet.Cells["A1:G1"].Value = data.Title;
                worksheet.Cells["A1:G1"].Style.Font.Size = 14;
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:G1"].Merge = true;
                worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                worksheet.Cells["A2:G2"].Value = dateToDate;
                worksheet.Cells["A2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A3:G3"].Value = "";
                worksheet.Cells["A4:A4"].Value = data.ColumnTitle;
                worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                worksheet.Cells["A4:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:G4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["B4:G4"].Value = "Doanh thu";
                worksheet.Cells["B4:G4"].Merge = true;
                worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells["B4:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                var row = 5;
                foreach (var item in data.Data)
                {
                    worksheet.Cells[row, 1].Value = !string.IsNullOrEmpty(item.PartnerName) ? item.PartnerName : "Không xác định";
                    worksheet.Cells[row, 2, row, 7].Value = item.PriceSubTotal;
                    worksheet.Cells[row, 2, row, 7].Style.Numberformat.Format = "#,###,###";
                    worksheet.Cells[row, 2, row, 7].Merge = true;
                    row++;
                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "Ngày thanh toán";
                    worksheet.Cells[row, 3].Value = "Số phiếu";
                    worksheet.Cells[row, 4].Value = "Bác sĩ";
                    worksheet.Cells[row, 5].Value = "Phụ tá";
                    worksheet.Cells[row, 6].Value = "Dịch vụ/Thuốc";
                    worksheet.Cells[row, 7].Value = "Thanh toán";
                    worksheet.Cells[row, 2, row, 7].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 2, row, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
                    var rowEnd = row + item.Lines.Count();
                    worksheet.Cells[row, 1, rowEnd, 1].Merge = true;
                    row++;

                    foreach (var line in item.Lines)
                    {
                        worksheet.Cells[row, 1].Value = "";
                        worksheet.Cells[row, 2].Value = line.InvoiceDate;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/MM/YYYY";
                        worksheet.Cells[row, 3].Value = line.InvoiceOrigin;
                        worksheet.Cells[row, 4].Value = line.EmployeeName;
                        worksheet.Cells[row, 5].Value = line.AssistantName;
                        worksheet.Cells[row, 6].Value = line.ProductName;
                        worksheet.Cells[row, 7].Value = line.PriceSubTotal;
                        worksheet.Cells[row, 7].Style.Numberformat.Format = "#,###,###";
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
    }
}
