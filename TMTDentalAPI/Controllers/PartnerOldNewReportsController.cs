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
    public class PartnerOldNewReportsController : ControllerBase
    {
        private IPartnerOldNewReportService _partnerOldNewReportService;
        private readonly IViewRenderService _viewRenderService;
        private IConverter _converter;
        public PartnerOldNewReportsController(IPartnerOldNewReportService partnerOldNewReportService, IViewRenderService viewRenderService,
            IConverter converter)
        {
            _partnerOldNewReportService = partnerOldNewReportService;
            _viewRenderService = viewRenderService;
            _converter = converter;
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
        public async Task<IActionResult> SumReport([FromQuery] PartnerOldNewReportSumReq val)
        {
            var res = await _partnerOldNewReportService.SumReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SumReVenue([FromQuery] PartnerOldNewReportSumReq val)
        {
            var res = await _partnerOldNewReportService.SumReVenue(val);
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
    }
}
