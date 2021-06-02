using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsMessageDetailsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISmsMessageDetailService _smsMessageDetailService;
        public SmsMessageDetailsController(IMapper mapper, ISmsMessageDetailService smsMessageDetailService)
        {
            _mapper = mapper;
            _smsMessageDetailService = smsMessageDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedResult([FromQuery] SmsMessageDetailPaged val)
        {
            var res = await _smsMessageDetailService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPagedStatistic([FromQuery] SmsMessageDetailPaged val)
        {
            var res = await _smsMessageDetailService.GetPagedStatistic(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ReSend(IEnumerable<Guid> ids)
        {
            var details = await _smsMessageDetailService.SearchQuery().Where(x => ids.Contains(x.Id) && x.CompanyId == CompanyId).Include(x => x.SmsAccount).ToListAsync();
            if (details != null && details.Any())
                await _smsMessageDetailService.ReSendSms(details);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> RunJobSendSms()
        {
            await _smsMessageDetailService.RunJobSendSms();
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReportTotal([FromQuery] ReportTotalInput val)
        {
            var res = await _smsMessageDetailService.GetReportTotal(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReportCampaign([FromQuery] ReportCampaignPaged val)
        {
            var res = await _smsMessageDetailService.GetReportCampaign(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetReportSupplierSumaryChart(ReportSupplierPaged val)
        {
            var res = await _smsMessageDetailService.GetReportSupplierSumary(val);
            return Ok(res);
        }
    }
}
