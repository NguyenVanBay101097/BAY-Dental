using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountFinancialReportsController : ControllerBase
    {
        private readonly IAccountFinancialReportService _accountFinancialReportService;
        private readonly IMapper _mapper;
        public AccountFinancialReportsController(IAccountFinancialReportService accountFinancialReportService, IMapper mapper)
        {
            _accountFinancialReportService = accountFinancialReportService;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProfitAndLossReport()
        {
            var res =await _accountFinancialReportService.GetProfitAndLossReport();
            var basic = _mapper.Map<AccountFinancialReportBasic>(res);
            return Ok(basic);
        }
    }
}