using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionSettlementsController : BaseApiController
    {
        private readonly ICommissionSettlementService _commissionSettlementService;
        private readonly IExportExcelService _exportExcelService;
        public CommissionSettlementsController(ICommissionSettlementService commissionSettlementService, IExportExcelService exportExcelService)
        {
            _commissionSettlementService = commissionSettlementService;
            _exportExcelService = exportExcelService;
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Commission")]
        public async Task<IActionResult> GetReport(CommissionSettlementReport val)
        {
            var result = await _commissionSettlementService.GetReport(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Report.Commission")]
        public async Task<IActionResult> GetReportPaged([FromQuery] CommissionSettlementReport val)
        {
            var result = await _commissionSettlementService.GetReportPaged(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Commission")]
        public async Task<IActionResult> GetReportDetail(CommissionSettlementDetailReportPar val)
        {
            var result = await _commissionSettlementService.GetReportDetail(val);
            return Ok(result);
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.Commission")]
        public async Task<IActionResult> GetSumReport(CommissionSettlementReport val)
        {
            var result = await _commissionSettlementService.GetSumReport(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcel([FromQuery] CommissionSettlementReport val)
        {
            var data = await _commissionSettlementService.ExportExcel(val);
            var listTitle = new List<string>() {
            "Nhân viên",
            "Loại hoa hồng",
            "Tiền hoa hồng"
            };
            await _exportExcelService.CreateAndAddToHeader(data, "Hoa hồng nhân viên", listTitle);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DetailExportExcel([FromQuery] CommissionSettlementDetailReportPar val)
        {
            var data = await _commissionSettlementService.DetailExportExcel(val);
            var listTitle = new List<string>() {
            "Ngày thanh toán",
            "Nhân viên",
            "Loại hoa hồng",
            "Số phiếu",
            "Khách hàng",
            "Dịch vụ",
            "Lợi nhuận tính hoa hồng",
            "% Hoa hồng",
            "Tiền hoa hồng"
            };
            await _exportExcelService.CreateAndAddToHeader(data, "Hoa hồng chi tiet nhân viên", listTitle);
            return Ok();
        }

    }
}
