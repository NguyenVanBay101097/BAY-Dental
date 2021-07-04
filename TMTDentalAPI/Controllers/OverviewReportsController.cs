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
    public class OverviewReportsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IOverviewReportService _overviewReportService;

        public OverviewReportsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDefaultCustomerReceipt([FromQuery] GetDefaultRequest val)
        {
            var res = await _overviewReportService.GetDefaultCustomerReceipt(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCount()
        {
            var result = await _overviewReportService.GetCountMedicalXaminationToday();
            return Ok(result);
        }

        [HttpPost("[action]")]      
        public async Task<IActionResult> CreateCustomerReceiptToAppointment(CustomerReceiptRequest val)
        {
           await _overviewReportService.CreateCustomerReceiptToAppointment(val);
            return NoContent();
        }

      
    }
}
