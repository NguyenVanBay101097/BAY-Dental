using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
        private readonly ICompanyService _companyService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public AccountCommonPartnerReportsController(IAccountCommonPartnerReportService reportService, 
            IViewRenderService viewRenderService, IConverter converter, ICompanyService companyService, 
            UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _reportService = reportService;
            _viewRenderService = viewRenderService;
            _converter = converter;
            _companyService = companyService;
            _userManager = userManager;
            _mapper = mapper;
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetPartnerReportSumaryOverview(PartnerQueryableFilter val)
        {
            var res = await _reportService.GetPartnerReportSumaryOverview(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetPartnerReportTreeMapOverview(PartnerQueryableFilter val)
        {
            var res = await _reportService.GetPartnerReportTreeMapOverview(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetPartnerReportSourceOverview(PartnerQueryableFilter val)
        {
            var res = await _reportService.GetPartnerReportSourceOverview(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> GetPartnerReportGenderOverview(PartnerQueryableFilter val)
        {
            var res = await _reportService.GetPartnerReportGenderOverview(val);
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
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoCongNo_NCC");

                worksheet.Cells["A1:G1"].Value = "BÁO CÁO CÔNG NỢ NHÀ CUNG CẤP";
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells["A1:G1"].Style.Font.Size = 14;
                worksheet.Cells["A1:G1"].Merge = true;
                worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));

                worksheet.Cells["A2:G2"].Value = @$"{(val.FromDate.HasValue ? "Từ ngày " + val.FromDate.Value.ToShortDateString() : "")}  {(val.ToDate.HasValue ? "đến ngày " + val.ToDate.Value.ToShortDateString() : "")}";
                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheet.Cells[4, 1].Value = val.ResultSelection == "customer" ? "Khách hàng" : "Nhà cung cấp";
                worksheet.Cells[4, 2].Value = val.ResultSelection == "customer" ? "Mã KH" : "Mã NCC";
                worksheet.Cells[4, 3].Value = "Số điện thoại";
                worksheet.Cells[4, 4].Value = "Nợ đầu kỳ";
                worksheet.Cells[4, 5].Value = "Phát sinh";
                worksheet.Cells[4, 6].Value = "Thanh toán";
                worksheet.Cells[4, 7].Value = "Nợ cuối kỳ";

                worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                worksheet.Cells["A4:G4"].Style.Font.Size = 14;
                worksheet.Cells["A4:G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells["A4:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:G4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A4:G4"].Style.Font.Color.SetColor(Color.White);
                var row = 5;
                foreach (var item in res)
                {
                    worksheet.Cells[row, 1].Value = item.PartnerName;
                    worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 2].Value = item.PartnerRef;
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 3].Value = item.PartnerPhone;
                    worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 4].Value = item.Begin;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 5].Value = item.Debit;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 6].Value = item.Credit;
                    worksheet.Cells[row, 6].Style.Numberformat.Format ="#,##0";
                    worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 7].Value = item.End;
                    worksheet.Cells[row, 7].Style.Numberformat.Format ="#,##0";
                    worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
        public async Task<IActionResult> ReportPartnerAdvance(ReportPartnerAdvanceFilter val)// khách hàng tạm ứng
        {
            var res = await _reportService.ReportPartnerAdvance(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> ReportPartnerAdvanceDetail(ReportPartnerAdvanceDetailFilter val)// chi tiết khách hàng tạm ứng
        {
            var res = await _reportService.ReportPartnerAdvanceDetail(val);
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
        public async Task<IActionResult> GetReportPartnerAdvancePdf(ReportPartnerAdvanceFilter val)
        {
            var data = await _reportService.ReportPartnerAdvance(val);
            var res = new ReportPartnerAdvancePrintVM();
            res.DateFrom = val.DateFrom;
            res.DateTo = val.DateTo;
            res.Company = val.CompanyId.HasValue ? _mapper.Map<CompanyPrintVM>(await _companyService.SearchQuery(x => x.Id == val.CompanyId).Include(x => x.Partner).FirstOrDefaultAsync()) : null;
            res.User = _mapper.Map<ApplicationUserSimple>(await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId));
            res.ReportPartnerAdvances = data;

            var html = _viewRenderService.Render("AccountCommonPartnerReport/ReportPartnerAdvancePdf", res);

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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo tạm ứng", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "BaoCaoTamUng.pdf");
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.AccountPartner")]
        public async Task<IActionResult> ExportReportPartnerDebitExcel(ReportPartnerDebitReq val)// công nợ khách hàng
        {
            var data = await _reportService.ExportReportPartnerDebitExcel(val);
            var dateToDate = "";
            if (val.FromDate.HasValue && val.ToDate.HasValue)
            {
                dateToDate = $"Từ ngày {val.FromDate.Value.ToString("dd/MM/yyyy")} đến ngày {val.ToDate.Value.ToString("dd/MM/yyyy")}";
            }
            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("ThongKeKhachHangDieuTri");

                worksheet.Cells["A1:H1"].Value = "BÁO CÁO CÔNG NỢ KHÁCH HÀNG";
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
                worksheet.Cells["B4:D4"].Merge = true;
                worksheet.Cells["B4:D4"].Value = "Số điện thoại";
                worksheet.Cells["B4:D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["E4"].Value = "Nợ đầu kỳ";
                worksheet.Cells["F4"].Value = "Phát sinh";
                worksheet.Cells["G4"].Value = "Thanh toán";
                worksheet.Cells["H4"].Value = "Nợ cuối kỳ";
                worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                worksheet.Cells["A4:H4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:H4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A4:H4"].Style.Font.Color.SetColor(Color.White);

                var row = 5;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = !string.IsNullOrEmpty(item.PartnerDisplayName) ? item.PartnerDisplayName : "Không xác định";
                    worksheet.Cells[row, 2, row, 4].Value = item.PartnerPhone;
                    worksheet.Cells[row, 2, row, 4].Merge = true;
                    worksheet.Cells[row, 2, row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "@";

                    worksheet.Cells[row, 5].Value = item.Begin;
                    worksheet.Cells[row, 6].Value = item.Debit;
                    worksheet.Cells[row, 7].Value = item.Credit;
                    worksheet.Cells[row, 8].Value = item.End;
                    worksheet.Cells[row, 5, row, 8].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[row, 1, row, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    row++;
                    if (!item.Lines.Any())
                        continue;

                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "Ngày";
                    worksheet.Cells[row, 3].Value = "Số phiếu";
                    worksheet.Cells[row, 4].Value = "Nội dung";
                    worksheet.Cells[row, 5].Value = "Nợ đầu kỳ";
                    worksheet.Cells[row, 6].Value = "Phát sinh";
                    worksheet.Cells[row, 7].Value = "Thanh toán";
                    worksheet.Cells[row, 8].Value = "Nợ cuối kỳ";
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 2, row, 8].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
                    worksheet.Cells[row, 2, row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    var rowEnd = row + item.Lines.Count();
                    worksheet.Cells[row, 1, rowEnd, 1].Merge = true;
                    row++;

                    foreach (var line in item.Lines)
                    {
                        worksheet.Cells[row, 1].Value = "";
                        worksheet.Cells[row, 2].Value = line.Date;
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/mm/yyyy";
                        worksheet.Cells[row, 3].Value = line.InvoiceOrigin;
                        worksheet.Cells[row, 4].Value = line.Ref;
                        worksheet.Cells[row, 5].Value = line.Begin;
                        worksheet.Cells[row, 6].Value = line.Debit;
                        worksheet.Cells[row, 7].Value = line.Credit;
                        worksheet.Cells[row, 8].Value = line.End;
                        worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 4, row, 8].Style.Numberformat.Format = "#,##0";
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
        public async Task<IActionResult> ExportReportPartnerAdvanceExcel(ReportPartnerAdvanceFilter val)
        {
            var stream = new MemoryStream();
            var data = await _reportService.ReportPartnerAdvance(val);
            byte[] fileContent;
            var dateToDate = "";
            if (val.DateFrom.HasValue && val.DateTo.HasValue)
            {
                dateToDate = $"Từ ngày {val.DateFrom.Value.ToString("dd/MM/yyyy")} đến ngày {val.DateTo.Value.ToString("dd/MM/yyyy")}";
            }

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoTamUng");

                worksheet.Cells["A1:I1"].Value = "BÁO CÁO TẠM ỨNG";
                worksheet.Cells["A1:I1"].Style.Font.Size = 14;
                worksheet.Cells["A1:I1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:I1"].Merge = true;
                worksheet.Cells["A1:I1"].Style.Font.Bold = true;
                worksheet.Cells["A2:I2"].Value = dateToDate;
                worksheet.Cells["A2:I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:I2"].Merge = true;
                worksheet.Cells["A3:I3"].Value = "";
                worksheet.Cells["A4:C4"].Value = "Khách hàng";
                worksheet.Cells["A4:C4"].Merge = true;
                worksheet.Cells["D4"].Value = "Số điện thoại";
                worksheet.Cells["E4"].Value = "Dư đầu kỳ";
                worksheet.Cells["F4"].Value = "Đã đóng";
                worksheet.Cells["G4"].Value = "Đã dùng";
                worksheet.Cells["H4"].Value = "Đã hoàn";
                worksheet.Cells["I4"].Value = "Dư cuối kỳ";
                worksheet.Cells["A4:I4"].Style.Font.Bold = true;
                worksheet.Cells["A4:I4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A4:I4"].Style.Font.Color.SetColor(Color.White);
              
                var row = 5;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = !string.IsNullOrEmpty(item.PartnerDisplayName) ? item.PartnerDisplayName : "Không xác định";
                    worksheet.Cells[row, 1, row, 3].Merge = true;
                    worksheet.Cells[row, 1, row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[row, 4].Value = item.PartnerPhone;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "@";

                    worksheet.Cells[row, 5].Value = item.Begin;
                    worksheet.Cells[row, 6].Value = item.Debit;
                    worksheet.Cells[row, 7].Value = item.Credit;
                    worksheet.Cells[row, 8].Value = item.Refund;
                    worksheet.Cells[row, 9].Value = item.End;
                    worksheet.Cells[row, 5, row, 9].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[row, 1, row, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    row++;

                    var childs = await _reportService.ReportPartnerAdvanceDetail(new ReportPartnerAdvanceDetailFilter
                    {
                        CompanyId = val.CompanyId,
                        DateFrom = val.DateFrom,
                        DateTo = val.DateTo,
                        PartnerId = item.PartnerId
                    });

                    if (childs.Any())
                    {
                        worksheet.Cells[row, 2].Value = "Ngày";
                        worksheet.Cells[row, 3].Value = "Số phiếu";
                        worksheet.Cells[row, 4].Value = "Nội dung";
                        worksheet.Cells[row, 5].Value = "Dư đầu kỳ";
                        worksheet.Cells[row, 6].Value = "Phát sinh";
                        worksheet.Cells[row, 7].Value = "Thanh toán";
                        worksheet.Cells[row, 7, row, 8].Merge = true;
                        worksheet.Cells[row, 9].Value = "Dư cuối kỳ";
                        worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 2, row, 9].Style.Font.Bold = true;
                        worksheet.Cells[row, 2, row, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 2, row, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
                        worksheet.Cells[row, 2, row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        var rowEnd = row + childs.Count();
                        worksheet.Cells[row, 1, rowEnd, 1].Merge = true;
                        row++;

                        foreach (var line in childs)
                        {
                            worksheet.Cells[row, 2].Value = line.Date;
                            worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/mm/yyyy";
                            worksheet.Cells[row, 3].Value = line.InvoiceOrigin;
                            worksheet.Cells[row, 4].Value = line.Name;
                            worksheet.Cells[row, 5].Value = line.Begin;
                            worksheet.Cells[row, 6].Value = line.Debit;
                            worksheet.Cells[row, 7].Value = line.Credit;
                            worksheet.Cells[row, 7, row, 8].Merge = true;
                            worksheet.Cells[row, 9].Value = line.End;
                            worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 4, row, 9].Style.Numberformat.Format = "#,##0";
                            row++;
                        }
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