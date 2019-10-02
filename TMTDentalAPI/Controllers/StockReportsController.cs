using System;
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
    public class StockReportsController : BaseApiController
    {
        private readonly IStockReportService _stockReportService;
        public StockReportsController(IStockReportService stockReportService)
        {
            _stockReportService = stockReportService;
        }

        [HttpPost("XuatNhapTonSummary")]
        public async Task<IActionResult> XuatNhapTonSummary(StockReportXuatNhapTonSearch val)
        {
            var res = await _stockReportService.XuatNhapTonSummary(val);
            return Ok(res);
        }

        [HttpPost("XuatNhapTonDetail")]
        public async Task<IActionResult> XuatNhapTonDetail(StockReportXuatNhapTonItem val)
        {
            var res = await _stockReportService.XuatNhapTonDetail(val);
            return Ok(res);
        }
    }
}