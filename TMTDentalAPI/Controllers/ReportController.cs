using ApplicationCore.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BaseApiController
    {
        private readonly IReportRenderService _reportRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        public ReportController(IReportRenderService reportRenderService,
            IPrintTemplateConfigService printTemplateConfigService)
        {
            _reportRenderService = reportRenderService;
            _printTemplateConfigService = printTemplateConfigService;
        }

        [AllowAnonymous]
        [HttpGet("{converter}/{reportName}/{ids}")]
        public async Task<IActionResult> ReportDownload(string converter, string reportName, string ids)
        {
            PrintTemplateConfig report = _printTemplateConfigService.SearchQuery(x => x.Type == reportName)
                .Include(x => x.PrintTemplate)
                .Include(x => x.PrintPaperSize)
                .FirstOrDefault();
            if (report == null)
                return NotFound();

            var docIds = !ids.IsNullOrWhiteSpace() ? ids.Split(",").Select(x => Guid.Parse(x)) : new List<Guid>();

            if (converter == "html")
            {
                var html = await _reportRenderService.RenderHtml(report, docIds);
                return new ContentResult
                {
                    Content = html,
                    ContentType = "text/html",
                };
            }
            else if (converter == "pdf")
            {
                var pdf = await _reportRenderService.RenderPdf(report, docIds);
                return new FileContentResult(pdf, "application/pdf");
            }
            else
            {
                throw new Exception("Not support");
            }
        }
    }
}
