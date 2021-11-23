using ApplicationCore.Utilities;
using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResInsuranceReportsController : BaseApiController
    {
        private readonly IResInsuranceReportService _resInsuranceReportService;
        private readonly IViewRenderService _viewRenderService;
        private IConverter _converter;
        private readonly IMapper _mapper;

        public ResInsuranceReportsController(IResInsuranceReportService resInsuranceReportService, IViewRenderService viewRenderService, IConverter converter, IMapper mapper)
        {
            _resInsuranceReportService = resInsuranceReportService;
            _viewRenderService = viewRenderService;
            _converter = converter;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetInsuranceDebtReport([FromQuery] InsuranceDebtFilter val)
        {
            var result = await _resInsuranceReportService.GetInsuranceDebtReport(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSummaryReports(InsuranceReportFilter val)
        {
            var res = await _resInsuranceReportService.ReportSummary(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDetailReports(InsuranceReportDetailFilter val)
        {
            var res = await _resInsuranceReportService.ReportDetail(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSummaryPdf(InsuranceReportFilter val)
        {
            var data = await _resInsuranceReportService.ReportSummaryPrint(val);
            var html = _viewRenderService.Render("ResInsurance/GetSummaryPdf", data);

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
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Báo cáo công nợ bảo hiểm", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "BaoCaoCongNo_BH.pdf");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelFile(InsuranceDebtFilter val)
        {
            var res = await _resInsuranceReportService.GetInsuranceDebtReport(val);

            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("CongNoBaoHiem");

                //worksheet.Cells["A1:G1"].Value = "BÁO CÁO CÔNG NỢ BẢO HIỂM";
                //worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Blue);
                //worksheet.Cells["A1:G1"].Style.Font.Size = 14;
                //worksheet.Cells["A1:G1"].Merge = true;
                //worksheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //worksheet.Cells["A1:G1"].Style.Font.Bold = true;
                //worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));

                //worksheet.Cells["A2:G2"].Value = @$"{(val.DateFrom.HasValue ? "Từ ngày " + val.DateFrom.Value.ToShortDateString() : "")}  {(val.DateTo.HasValue ? "đến ngày " + val.DateTo.Value.ToShortDateString() : "")}";
                //worksheet.Cells["A2:G2"].Merge = true;
                //worksheet.Cells["A2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;



                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Nguồn";
                worksheet.Cells[1, 3].Value = "Nội dung";
                worksheet.Cells[1, 4].Value = "Số tiền";


                worksheet.Cells["A1:D1"].Style.Font.Bold = true;
                worksheet.Cells["A1:D1"].Style.Font.Size = 11;
                worksheet.Cells["A1:D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:D1"].Style.Border.Bottom.Color.SetColor(Color.White);
                worksheet.Cells["A1:D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:D1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A1:D1"].Style.Font.Color.SetColor(Color.White);
                var row = 2;
                foreach (var item in res)
                {
                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2].Value = item.Origin;
                    worksheet.Cells[row, 3].Value = "";
                    worksheet.Cells[row, 4].Value = item.AmountTotal;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";

                    worksheet.Cells[row, 2, row, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 2, row, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 2, row, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 2, row, 4].Style.Border.Bottom.Color.SetColor(Color.White);
                    worksheet.Cells[row, 2, row, 4].Style.Font.Size = 11;

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

      

    }
}
