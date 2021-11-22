using AutoMapper;
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
    public class ResInsuranceReportsController : BaseApiController
    {
        private readonly IResInsuranceReportService _resInsuranceReportService;
        private readonly IMapper _mapper;

        public ResInsuranceReportsController(IResInsuranceReportService resInsuranceReportService, IMapper mapper)
        {
            _resInsuranceReportService = resInsuranceReportService;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetInsuranceDebtReport([FromQuery] InsuranceDebtFilter val)
        {
            var result = await _resInsuranceReportService.GetInsuranceDebtReport(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSummaryReports(InsuranceReportFilter val)
        {
            var res = await _resInsuranceReportService.ReportSummary(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDetailReports(InsuranceReportDetailFilter val)
        {
            var res = await _resInsuranceReportService.ReportDetail(val);
            return Ok(res);
        }

    }
}
