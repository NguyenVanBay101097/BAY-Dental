using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
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
        public async Task<IActionResult> ExportExcel([FromQuery] CommissionSettlementReportExportExcelPar val)
        {
            var data = await _commissionSettlementService.ExportExcelData(val);
            decimal sum = data.Sum(x => x.Amount.Value);
            var listTitle = new List<string>() {
            "Nhân viên",
            "Loại hoa hồng",
            "Tiền hoa hồng"
            };
            var packageByte = await _exportExcelService.createExcel(data, "Hoa hồng nhân viên", listTitle) as byte[];
            var package = _exportExcelService.ConverByteArrayTOExcepPackage(packageByte) as ExcelPackage;
            using (package)
            {
                var worksheet = package.Workbook.Worksheets[0];
                worksheet.Cells[data.Count() + 2, 1].Value = "Tổng";
                worksheet.Cells[data.Count() + 2, 2, data.Count() + 2, worksheet.Dimension.End.Column].Merge = true;
                worksheet.Cells[data.Count() + 2, 2].Value = sum;
                worksheet.Cells[data.Count() + 2, 2].Style.Numberformat.Format = "0,00";
                await _exportExcelService.AddToHeader(package.GetAsByteArray());
            }
            return Ok();

        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DetailExportExcel([FromQuery] CommissionSettlementDetailReportExcelPar val)
        {
            var data = await _commissionSettlementService.DetailExportExcelData(val);
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
            var packageByte = await _exportExcelService.createExcel(data, "Hoa hồng chi tiết nhân viên", listTitle) as byte[];
            var package = _exportExcelService.ConverByteArrayTOExcepPackage(packageByte) as ExcelPackage;
            using (package)
            {
                var worksheet = package.Workbook.Worksheets[0];
                worksheet.Cells[2, 8, worksheet.Dimension.End.Row, 8].Style.Numberformat.Format = "#0\\%";
                await _exportExcelService.AddToHeader(package.GetAsByteArray());
            }
            return Ok();
        }

    }
}
