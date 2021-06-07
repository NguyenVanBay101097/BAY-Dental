using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportCustomerDebtsController : BaseApiController
    {
        private readonly IReportCustomerDebtService _reportCustomerDebtService;

        public ReportCustomerDebtsController(IReportCustomerDebtService reportCustomerDebtService)
        {
            _reportCustomerDebtService = reportCustomerDebtService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReports([FromQuery] CustomerDebtFilter val)
        {
            var result = await _reportCustomerDebtService.GetPagedtCustomerDebtReports(val);
            return Ok(result);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetAmountDebtTotal([FromQuery] AmountCustomerDebtFilter val)
        {
            var res = await _reportCustomerDebtService.GetCustomerAmountTotal(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcelFile([FromQuery] CustomerDebtFilter val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            var data = await _reportCustomerDebtService.GetPagedtCustomerDebtReports(val);
            byte[] fileContent;
            var sheetName = "Sổ công nợ khách hàng";


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Nguồn";
                worksheet.Cells[1, 3].Value = "Nội dung";
                worksheet.Cells[1, 4].Value = "Số tiền";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.InvoiceOrigin;
                    worksheet.Cells[row, 3].Value = item.Name;
                    worksheet.Cells[row, 4].Value = item.Balance;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,###";

                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }
    }
}
