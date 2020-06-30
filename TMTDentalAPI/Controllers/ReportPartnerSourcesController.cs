using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportPartnerSourcesController : BaseApiController
    {
        private readonly IPartnerSourceService _sourceService;

        public ReportPartnerSourcesController(IPartnerSourceService sourceService)
        {
            _sourceService = sourceService;
        }

        [HttpPost]
        public async Task<IActionResult> GetReport([FromQuery]ReportFilterPartnerSource val)
        {
            var res = await _sourceService.GetReportPartnerSource(val);
            return Ok(res);
        }

    }
}