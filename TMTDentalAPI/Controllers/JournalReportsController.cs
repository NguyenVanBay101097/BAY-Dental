using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    public class JournalReportsController : BaseApiController
    {
        private readonly IJournalReportService _journalReportService;

        public JournalReportsController(IJournalReportService journalReportService)
        {
            _journalReportService = journalReportService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]JournalReportPaged paged)
        {
            var res = _journalReportService.GetJournalMoveLineReport(paged);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public IActionResult GetMoveLines(JournalReportDetailPaged val)
        {
            var res = _journalReportService.GetMoveLines(val);
            return Ok(res);
        }

    }
}