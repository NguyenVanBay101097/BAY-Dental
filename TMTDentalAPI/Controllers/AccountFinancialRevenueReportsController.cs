using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountFinancialRevenueReportsController : BaseApiController
    {
        private readonly IAccountFinancialRevenueReportService _AccountFinancialRevenueReportService;
        private readonly IMapper _mapper;
        public AccountFinancialRevenueReportsController(IAccountFinancialRevenueReportService AccountFinancialRevenueReportService, IMapper mapper)
        {
            _AccountFinancialRevenueReportService = AccountFinancialRevenueReportService;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Financial")]
        public async Task<IActionResult> GetRevenueReport(RevenueReportPar val)//bao cao nguon thu
        {
            var res = await _AccountFinancialRevenueReportService.getRevenueReport(val);
            return Ok(res);
        }
    }
}