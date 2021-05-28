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
        public async Task<IActionResult> GetRevenueReportPaged([FromQuery] AccountInvoiceReportPaged val)
        {
            var res = await _invoiceReportService.GetRevenueReportPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetRevenueReportDetailPaged([FromQuery] AccountInvoiceReportDetailPaged val)
        {
            var res = await _invoiceReportService.GetRevenueReportDetailPaged(val);
            return Ok(res);
        }

    }
}