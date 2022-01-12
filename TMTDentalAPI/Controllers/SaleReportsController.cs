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
using Kendo.DynamicLinqCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleReportsController : BaseApiController
    {
        private readonly ISaleReportService _saleReportService;
        private readonly ISaleOrderLineService _saleOrderLineService;
        private readonly IViewRenderService _viewRenderService;
        private IConverter _converter;

        public SaleReportsController(ISaleReportService saleReportService, IViewRenderService viewRenderService,
            IConverter converter,
            ISaleOrderLineService saleOrderLineService)
        {
            _saleReportService = saleReportService;
            _saleOrderLineService = saleOrderLineService;
            _viewRenderService = viewRenderService;
            _converter = converter;
        }

        [HttpGet("GetTopService")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetTopService([FromQuery] SaleReportTopServicesFilter val)
        {
            var res = await _saleReportService.GetTopServices(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetReport(SaleReportSearch val)
        {
            var res = await _saleReportService.GetReport(val);
            return Ok(res);
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetReportService(SaleReportSearch val)
        {
            var res = await _saleReportService.GetReportService(val.DateFrom, val.DateTo, val.CompanyId, val.Search, val.State);
            return Ok(res);
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetTopSaleProduct(SaleReportTopSaleProductSearch val)
        {
            var res = await _saleReportService.GetTopSaleProduct(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetReportDetail(SaleReportItem val)
        {
            var res = await _saleReportService.GetReportDetail(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.PartnerOldNew")]
        public async Task<IActionResult> GetReportPartner(SaleReportPartnerSearch val)
        {
            //var res = await _saleReportService.GetReportPartner(val);
            //var res = await _saleReportService.GetReportPartnerV2(val);
            var res = await _saleReportService.GetReportPartnerV3(val);
            //var res = await _saleReportService.GetReportPartnerV4(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> ExportServiceReportExcelFile(SaleReportSearch val)
        {
            var stream = new MemoryStream();
            var data = await _saleReportService.GetReportService(val.DateFrom, val.DateTo, val.CompanyId, val.Search, val.State);
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoDichVuTrongNgay");

                worksheet.Cells["A1:K1"].Value = "BÁO CÁO DỊCH VỤ TRONG NGÀY";
                worksheet.Cells["A1:K1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells["A1:K1"].Style.Font.Size = 20;
                worksheet.Cells["A1:K1"].Merge = true;

                worksheet.Cells["A2:K2"].Value = $"Ngày {val.DateFrom.Value.ToShortDateString()}";
                worksheet.Cells["A2:K2"].Style.Numberformat.Format = "dd/mm/yyyy";
                worksheet.Cells["A2:K2"].Merge = true;

                worksheet.Cells[4, 1].Value = "Dịch vụ";
                worksheet.Cells[4, 2].Value = "Phếu điều trị";
                worksheet.Cells[4, 3].Value = "Khách hàng";
                worksheet.Cells[4, 4].Value = "Số lượng";
                worksheet.Cells[4, 5].Value = "Bác sĩ";
                worksheet.Cells[4, 6].Value = "Đơn vị tính";
                worksheet.Cells[4, 7].Value = "Răng";
                worksheet.Cells[4, 8].Value = "Chuẩn đoán";

                worksheet.Cells[4, 9].Value = "Thành tiền";
                worksheet.Cells[4, 10].Value = "Thanh toán";
                worksheet.Cells[4, 11].Value = "Còn lại";
                worksheet.Cells[4, 12].Value = "Trạng thái";

                worksheet.Cells["A4:L4"].Style.Font.Bold = true;
                worksheet.Cells["A4:L4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                var row = 5;

                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.Product.Name;
                    worksheet.Cells[row, 2].Value = item.Order.Name;
                    worksheet.Cells[row, 3].Value = item.OrderPartner.DisplayName;
                    worksheet.Cells[row, 4].Value = item.ProductUOMQty;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "0";
                    worksheet.Cells[row, 5].Value = item.Employee != null ? item.Employee.Name : "";
                    worksheet.Cells[row, 6].Value = item.ProductUOM != null ? item.ProductUOM.Name : "";
                    worksheet.Cells[row, 7].Value = item.TeethDisplay;
                    worksheet.Cells[row, 8].Value = item.Diagnostic;
                    worksheet.Cells[row, 9].Value = item.PriceTotal;
                    worksheet.Cells[row, 9].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 10].Value = (item.AmountInvoiced);
                    worksheet.Cells[row, 10].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 11].Value = (item.PriceTotal) - (item.AmountInvoiced ?? 0);
                    worksheet.Cells[row, 11].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 12].Value = GetSaleOrderState(item.State);
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.PartnerOldNew")]
        public async Task<IActionResult> GetReportOldNewPartner(SaleReportOldNewPartnerInput val)
        {
            var res = await _saleReportService.GetReportOldNewPartner(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetSummary(GetSummarySaleReportRequest val)
        {
            var states = new string[] { "sale", "done" };
            var query = _saleOrderLineService.SearchQuery(x => (!x.Order.IsQuotation.HasValue || x.Order.IsQuotation == false) &&
                states.Contains(x.State));
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.OrderPartnerId == val.PartnerId);
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            var res = await query.GroupBy(x => 0).Select(x => new GetSummarySaleReportResponse
            {
                PriceTotal = x.Sum(s => s.PriceTotal),
                PaidTotal = x.Sum(x => x.AmountPaid),
                ResidualTotal = x.Sum(x => x.PriceTotal - x.AmountPaid),
                QtyTotal = x.Sum(s => s.ProductUOMQty)
            }).ToListAsync();

            return Ok(res.Count > 0 ? res[0] : new GetSummarySaleReportResponse());
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetServiceOverviewReport(ServiceReportReq val)
        {
            var res = await _saleReportService.ServiceOverviewReport(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> PrintPdfServiceOverviewReport(SaleOrderLinesPaged val)
        {
            var data = await _saleReportService.PrintServiceOverviewReport(val);
            var html = _viewRenderService.Render("SaleReport/ServiceOverviewReportPdf", data);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 5 },
                DocumentTitle = "PDF Report"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css", "print.css") },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo dịch vụ - Tổng quan", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "BaoCaoDichVu_TongQuan.pdf");
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetServiceReportByTime(ServiceReportReq val)
        {
            var res = await _saleReportService.GetServiceReportByTime(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetServiceReportByService(ServiceReportReq val)
        {
            var res = await _saleReportService.GetServiceReportByService(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetServiceReportDetailPaged([FromQuery] ServiceReportDetailReq val)
        {
            var res = await _saleReportService.GetServiceReportDetailPaged(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetServiceReportByServicePdf(ServiceReportReq val)
        {
            var data = await _saleReportService.ServiceReportByServicePrint(val);
            var html = _viewRenderService.Render("SaleReport/ServiceReportPdf", data);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 5 },
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
            return File(file, "application/pdf", "ServiceReportService.pdf");
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> GetServiceReportByTimePdf(ServiceReportReq val)
        {
            var data = await _saleReportService.ServiceReportByTimePrint(val);
            var html = _viewRenderService.Render("SaleReport/ServiceReportPdf", data);

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
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "BaoCaoDichVu_TheoTG.pdf");
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> ExportServiceReportByTimeExcel(ServiceReportReq val)
        {
            var data = await _saleReportService.ServiceReportByTimeExcel(val);
            return _saleReportService.ExportServiceReportExcel(data, val.DateFrom, val.DateTo, "time");
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> ExportServiceReportByServiceExcel(ServiceReportReq val)
        {
            var data = await _saleReportService.ServiceReportByServiceExcel(val);
            return _saleReportService.ExportServiceReportExcel(data, val.DateFrom, val.DateTo, "service");

        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Sale")]
        public async Task<IActionResult> ExportServiceOverviewReportExcel(SaleOrderLinesPaged val)
        {
            var stream = new MemoryStream();
            var data = await _saleOrderLineService.GetPagedResultAsync(val);
            var dateToDate = "";
            if (val.DateFrom.HasValue && val.DateTo.HasValue)
            {
                dateToDate = $"Từ ngày {val.DateFrom.Value.ToString("dd/MM/yyyy")} đến ngày {val.DateTo.Value.ToString("dd/MM/yyyy")}";
            }

            byte[] fileContent;


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoDichVu_TongQuan");

                worksheet.Cells["A1:J1"].Value = "BÁO CÁO TỔNG QUAN DỊCH VỤ";
                //worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells["A1:J1"].Style.Font.Size = 14;
                worksheet.Cells["A1:J1"].Merge = true;
                worksheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:J1"].Style.Font.Bold = true;
                worksheet.Cells["A1:J1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A2:J2"].Value = dateToDate;
                worksheet.Cells["A2:J2"].Merge = true;
                worksheet.Cells["A2:J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheet.Cells[4, 1].Value = "Ngày tạo";
                worksheet.Cells[4, 2].Value = "Dịch vụ";
                worksheet.Cells[4, 3].Value = "Phiếu điều trị";
                worksheet.Cells[4, 4].Value = "Khách hàng";
                worksheet.Cells[4, 5].Value = "Số lượng";
                worksheet.Cells[4, 6].Value = "Bác sĩ";
                worksheet.Cells[4, 7].Value = "Thành tiền";
                worksheet.Cells[4, 8].Value = "Thanh toán";
                worksheet.Cells[4, 9].Value = "Còn lại";
                worksheet.Cells[4, 10].Value = "Trạng thái";


                worksheet.Cells["A4:J4"].Style.Font.Bold = true;
                worksheet.Cells["A4:J4"].Style.Font.Size = 11;
                worksheet.Cells["A4:J4"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:J4"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:J4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:J4"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:J4"].Style.Border.Bottom.Color.SetColor(Color.White);
                worksheet.Cells["A4:J4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:J4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0667d1"));
                worksheet.Cells["A4:J4"].Style.Font.Color.SetColor(Color.White);

                var row = 5;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.OrderName;
                    worksheet.Cells[row, 4].Value = item.OrderPartnerName;
                    worksheet.Cells[row, 5].Value = item.ProductUOMQty;
                    worksheet.Cells[row, 6].Value = item.EmployeeName;
                    worksheet.Cells[row, 7].Value = item.PriceSubTotal;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 8].Value = item.AmountInvoiced;
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 9].Value = item.AmountResidual;
                    worksheet.Cells[row, 9].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 10].Value = item.StateDisplay;

                    worksheet.Cells[row, 1, row, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    row++;
                }

                dynamic aggregates = data.Aggregates;
                worksheet.Cells[row, 1, row, 6].Value = "Tổng";
                worksheet.Cells[row, 1, row, 6].Merge = true;    
                worksheet.Cells[row, 7].Value = aggregates.PriceSubTotal.sum; 
                worksheet.Cells[row, 8].Value = aggregates.AmountInvoiced.sum;
                worksheet.Cells[row, 9].Value = (aggregates.PriceSubTotal.sum - aggregates.AmountInvoiced.sum);
                worksheet.Cells[row, 10].Value = "";
                worksheet.Cells[row, 1, row, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[row, 1, row, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[row, 1, row, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[row, 1, row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[row, 1, row, 10].Style.Font.Bold = true;
                worksheet.Cells[row, 7, row, 9].Style.Numberformat.Format = "#,##0";

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);

        }

        private string GetToothType(string toothType)
        {
            if (toothType == "whole_jaw")
                return "Nguyên hàm";
            else if (toothType == "upper_jaw")
                return "Hàm trên";
            else
                return "Hàm dưới";
        }

        private string GetSaleOrderState(string state)
        {
            if (state == "done")
                return "Hoàn thành";
            else if (state == "sale")
                return "Đang điều trị";
            else if (state == "cancel")
                return "Ngừng điều trị";
            else
                return "";
        }


    }

}