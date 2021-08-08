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
    public class DashboardReportsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IDashboardReportService _dashboardService;

        public DashboardReportsController(IMapper mapper, IDashboardReportService dashboardService)
        {
            _mapper = mapper;
            _dashboardService = dashboardService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDefaultCustomerReceipt(GetDefaultRequest val)
        {
            var res = await _dashboardService.GetDefaultCustomerReceipt(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCountMedicalXamination(ReportTodayRequest val)
        {
            var result = await _dashboardService.GetCountMedicalXaminationToday(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCustomerReceiptToAppointment(CustomerReceiptRequest val)
        {
            var res = await _dashboardService.CreateCustomerReceiptToAppointment(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCountSaleOrder(ReportTodayRequest val)
        {
            var res = await _dashboardService.GetCountSaleOrder(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSumary(ReportTodayRequest val)
        {
            var res = await _dashboardService.GetSumary(val);
            return Ok(res);
        }


    }
}
