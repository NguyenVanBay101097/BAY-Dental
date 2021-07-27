using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public AccountInvoiceReportsController(IAccountInvoiceReportService invoiceReportService, IProductService productService,
            IViewRenderService viewRenderService)
        {
            _productService = productService;
            _invoiceReportService = invoiceReportService;
            _viewRenderService = viewRenderService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueTimeReportPaged([FromQuery] RevenueTimeReportPar val)
        {
            var res = await _invoiceReportService.GetRevenueTimeReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueServiceReportPaged([FromQuery] RevenueServiceReportPar val)
        {
            var res = await _invoiceReportService.GetRevenueServiceReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueEmployeeReportPaged([FromQuery] RevenueEmployeeReportPar val)
        {
            var res = await _invoiceReportService.GetRevenueEmployeeReport(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueReportDetailPaged([FromQuery] RevenueReportDetailPaged val)
        {
            var res = await _invoiceReportService.GetRevenueReportDetailPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueReportDetailPrint([FromQuery] RevenueReportDetailPaged val)
        {
            var res = await _invoiceReportService.GetRevenueReportDetailPaged(val);

            if (res.Items == null) return NotFound();
            var html = _viewRenderService.Render("AccountInvoiceReport/PrintRevenueReportDetail", res.Items);

            return Ok(new PrintData() { html = html });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SumRevenueReport([FromQuery]SumRevenueReportPar val)
        {
            var res = await _invoiceReportService.SumRevenueReport(val);
            return Ok(res);
        }    

    }
}