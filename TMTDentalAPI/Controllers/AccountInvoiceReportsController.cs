using System;
using System.Collections.Generic;
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

        [HttpPost("GetSummaryByTime")]
        public async Task<IActionResult> GetSummaryByTime(AccountInvoiceReportByTimeSearch val)
        {
            var res = await _invoiceReportService.GetSummaryByTime(val);
            return Ok(res);
        }

        [HttpPost("GetDetailByTime")]
        public async Task<IActionResult> GetDetailByTime(AccountInvoiceReportByTimeItem val)
        {
            var res = await _invoiceReportService.GetDetailByTime(val);
            return Ok(res);
        }

        [HttpPost("GetHomeTodaySummary")]
        public async Task<IActionResult> GetHomeTodaySummary()
        {
            var res = await _invoiceReportService.GetToDaySummary();
            return Ok(res);
        }

        [HttpPost("GetAmountResidualToday")]
        public async Task<IActionResult> GetAmountResidualToday()
        {
            var res = await _invoiceReportService.GetAmountResidualToday();
            return Ok(res);
        }

        [HttpGet("GetTop/{number}")]
        public async Task<IActionResult> GetTop(int number)
        {
            var res = await _invoiceReportService.GetTopServices(number);
            
            return Ok(res);
        }
    }
}