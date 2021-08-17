﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountCommonPartnerReportsController : BaseApiController
    {
        private readonly IAccountCommonPartnerReportService _reportService;
        private readonly IViewRenderService _viewRenderService;
        private IConverter _converter;
        public AccountCommonPartnerReportsController(IAccountCommonPartnerReportService reportService, 
            IViewRenderService viewRenderService, IConverter converter)
        {
            _reportService = reportService;
            _viewRenderService = viewRenderService;
            _converter = converter;
        }

        [HttpPost("GetSummaryPartner")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetSummaryPartner(AccountCommonPartnerReportSearchV2 val)
        {
            var res = await _reportService.ReportSumaryPartner(val);
            return Ok(res);
        }

        [HttpPost("GetSummary")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetSummary(AccountCommonPartnerReportSearch val)
        {
            var res = await _reportService.ReportSummary(val);
            return Ok(res);
        }

        [HttpPost("GetDetail")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetDetail(AccountCommonPartnerReportItem val)
        {
            var res = await _reportService.ReportDetail(val);
            return Ok(res);
        }

        [HttpPost("GetListReportPartner")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetListReportPartner(AccountCommonPartnerReportSearch val)
        {
            var res = await _reportService.GetListReportPartner(val);
            return Ok(res);
        }

        [HttpPost("GetSalaryReportEmployee")]
        [CheckAccess(Actions = "Salary.AccountCommonPartnerReport")]
        public async Task<IActionResult> GetSalaryReportEmployee(AccountCommonPartnerReportSearch val)
        {
            var res = await _reportService.ReportSalaryEmployee(val);
            return Ok(res);
        }

        [HttpPost("ReportSalaryEmployeeDetail")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> ReportSalaryEmployeeDetail(AccountCommonPartnerReportItem val)
        {
            var res = await _reportService.ReportSalaryEmployeeDetail(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelFile(AccountCommonPartnerReportSearch val)
        {
            var res = await _reportService.ReportSummary(val);

            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = val.ResultSelection == "customer" ? "Khách hàng" : "Nhà cung cấp";
                worksheet.Cells[1, 2].Value = val.ResultSelection == "customer" ? "Mã KH" : "Mã NCC";
                worksheet.Cells[1, 3].Value = "Số điện thoại";
                worksheet.Cells[1, 4].Value = "Nợ đầu kỳ";
                worksheet.Cells[1, 5].Value = "Phát sinh";
                worksheet.Cells[1, 6].Value = "Thanh toán";
                worksheet.Cells[1, 7].Value = "Nợ cuối kỳ";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in res)
                {
                    worksheet.Cells[row, 1].Value = item.PartnerName;
                    worksheet.Cells[row, 2].Value = item.PartnerRef;
                    worksheet.Cells[row, 3].Value = item.PartnerPhone;
                    worksheet.Cells[row, 4].Value = item.Begin;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,###0";
                    worksheet.Cells[row, 5].Value = item.Debit;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,###0";
                    worksheet.Cells[row, 6].Value = item.Credit;
                    worksheet.Cells[row, 6].Style.Numberformat.Format ="#,###0";
                    worksheet.Cells[row, 7].Value = item.End;
                    worksheet.Cells[row, 7].Style.Numberformat.Format ="#,###0";
                    row++;
                }

                worksheet.Column(8).Style.Numberformat.Format = "@";
                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> ReportPartnerDebit(ReportPartnerDebitReq val)// công nợ khách hàng
        {
            var res = await _reportService.ReportPartnerDebit(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> ReportPartnerDebitDetail(ReportPartnerDebitDetailReq val)// công nợ khách hàng chi tiết
        {
            var res = await _reportService.ReportPartnerDebitDetail(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> ReportPartnerDebitSummary(ReportPartnerDebitReq val)// công nợ khách hàng
        {
            var res = await _reportService.ReportPartnerDebitSummary(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetReportPartnerDebitPdf([FromQuery]ReportPartnerDebitReq val)
        {
            var data = await _reportService.PrintReportPartnerDebit(val);
            var html = _viewRenderService.Render("AccountCommonPartnerReport/ReportPartnerDebitPdf", data);

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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo công nợ khách hàng", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "BaoCaoCongNo_KH.pdf");
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetSummaryPdf([FromBody] AccountCommonPartnerReportSearch val)
        {
            var data = await _reportService.ReportSummaryPrint(val);
            var html = _viewRenderService.Render("AccountCommonPartnerReport/GetSummaryPdf", data);

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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo công nợ nhà cc", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "BaoCaoCongNo_NCC.pdf");
        }
    }
}