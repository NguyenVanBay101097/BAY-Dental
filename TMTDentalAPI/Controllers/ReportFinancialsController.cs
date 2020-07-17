﻿using System;
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
    public class ReportFinancialsController : ControllerBase
    {
        private readonly IReportFinancialService _reportFinancialServiceService;
        public ReportFinancialsController(IReportFinancialService reportFinancialServiceService)
        {
            _reportFinancialServiceService = reportFinancialServiceService;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAccountMoveLines(AccountingReport val)
        {
            var res = await _reportFinancialServiceService.GetAccountLines(val);
            return Ok(res);
        }
    }
}