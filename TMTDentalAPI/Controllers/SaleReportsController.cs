﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using DinkToPdf;
using DinkToPdf.Contracts;
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
            var res = await _saleReportService.GetReportService(val);
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
            var data = await _saleReportService.GetReportService(val);
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Phếu điều trị";
                worksheet.Cells[1, 2].Value = "Khách hàng";
                worksheet.Cells[1, 3].Value = "Bác sĩ";
                worksheet.Cells[1, 4].Value = "Răng";
                worksheet.Cells[1, 5].Value = "Chuẩn đoán";
                worksheet.Cells[1, 6].Value = "Dịch vụ";
                worksheet.Cells[1, 7].Value = "Thành tiền";
                worksheet.Cells[1, 8].Value = "Thanh toán";
                worksheet.Cells[1, 9].Value = "Còn lại";
                worksheet.Cells[1, 10].Value = "Trạng thái";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

                var row = 2;
                
                foreach (var item in data.Items)
                {
                    string numberTeeth = "";
                    worksheet.Cells[row, 1].Value = item.Order.Name;
                    worksheet.Cells[row, 2].Value = item.OrderPartner.DisplayName;
                    worksheet.Cells[row, 3].Value = item.Employee != null ? item.Employee.Name : "";
                    if (item.Teeth.Any())
                    {
                        foreach (var te in item.Teeth)
                        {
                            numberTeeth += te.Name +", ";
                        }
                    }
                    worksheet.Cells[row, 4].Value = numberTeeth;
                    worksheet.Cells[row, 5].Value = item.Diagnostic;
                    worksheet.Cells[row, 6].Value = item.Product.Name;
                    worksheet.Cells[row, 7].Value = item.PriceSubTotal;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "#,#";
                    worksheet.Cells[row, 8].Value = (item.PriceSubTotal - item.AmountResidual);
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "#,#";
                    worksheet.Cells[row, 9].Value = item.AmountResidual;
                    worksheet.Cells[row, 9].Style.Numberformat.Format = "#,#";
                    worksheet.Cells[row, 10].Value = item.State == "done" ? "Hoàn thành" : (item.State == "sale" ? "Đang điều trị" : "");
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
                Margins = new MarginSettings { Top = 5},
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
    }
}