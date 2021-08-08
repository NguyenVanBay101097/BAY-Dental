using System;
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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
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
            var res = await _invoiceReportService.GetRevenueTimeReport(val);
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
    }
}
