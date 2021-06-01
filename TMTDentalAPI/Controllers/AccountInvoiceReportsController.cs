using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public AccountInvoiceReportsController(IAccountInvoiceReportService invoiceReportService, IProductService productService)
        {
            _productService = productService;
            _invoiceReportService = invoiceReportService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueTimeReportPaged([FromQuery] RevenueTimeReportPaged val)
        {
            var res = await _invoiceReportService.GetRevenueTimeReportPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueServiceReportPaged([FromQuery] RevenueServiceReportPaged val)
        {
            var res = await _invoiceReportService.GetRevenueServiceReportPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueEmployeeReportPaged([FromQuery] RevenueEmployeeReportPaged val)
        {
            var res = await _invoiceReportService.GetRevenueEmployeeReportPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueReportDetailPaged([FromQuery] RevenueReportDetailPaged val)
        {
            var res = await _invoiceReportService.GetRevenueReportDetailPaged(val);
            return Ok(res);
        }

    }
}