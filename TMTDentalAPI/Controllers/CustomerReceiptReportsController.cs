﻿using Infrastructure.Services;
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

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCountCustomerReceipt(CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetCountCustomerReceipt(val);
            return Ok(res);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetCountCustomerReceiptNoTreatment(CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetCountCustomerReceiptNotreatment(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCount(CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetCountTime(val);
            return Ok(res);
        }

    }
}
