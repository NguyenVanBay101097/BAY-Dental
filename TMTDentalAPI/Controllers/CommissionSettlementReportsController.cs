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
    public class CommissionSettlementReportsController : BaseApiController
    {
        private readonly ICommissionSettlementService _commissionSettlementService;
        public CommissionSettlementReportsController(ICommissionSettlementService commissionSettlementService)
        {
            _commissionSettlementService = commissionSettlementService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReport(CommissionSettlementReport val)
        {
            var result = await _commissionSettlementService.GetReport(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReportDetail(CommissionSettlementReport val)
        {
            var result = await _commissionSettlementService.GetReportDetail(val);
            return Ok(result);
        }
        
    }
}
