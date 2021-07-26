using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerReceiptReportsController : BaseApiController
    {
        private readonly ICustomerReceiptReportService _customerReceiptReportService;
        private readonly IProductService _productService;
        public CustomerReceiptReportsController(ICustomerReceiptReportService customerReceiptReportService, IProductService productService)
        {
            _productService = productService;
            _customerReceiptReportService = customerReceiptReportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReportPaged([FromQuery] CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetPagedResultAsync(val);
            return Ok(res);
        }
    }
}
