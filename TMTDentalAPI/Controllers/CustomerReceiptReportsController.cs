using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerReceiptReportsController : BaseApiController
    {
        private readonly ICustomerReceiptReportService _customerReceiptReportService;
        private readonly IProductService _productService;
        public CustomerReceiptReportsController(ICustomerReceiptReportService customerReceiptReportService, IProductService productService)
        {
            _productService = productService;
            _customerReceiptReportService = customerReceiptReportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReportPaged([FromQuery] CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCountCustomerReceipt(CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetCountCustomerReceipt(val);
            return Ok(res);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetCountCustomerReceiptNoTreatment(CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetCountCustomerReceiptNotreatment(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCount(CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetCountTime(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelReportNoTreatment(CustomerReceiptReportFilter val)
        {
            val.Limit = int.MaxValue;
            var stream = new MemoryStream();
            var data = await _customerReceiptReportService.GetPagedResultAsync(val);
            byte[] fileContent;


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoTiepNhan_KhongDieuTri");

                worksheet.Cells["A1:G1"].Value = "BÁO CÁO TIẾP NHẬN KHÔNG ĐIỀU TRỊ";
                worksheet.Cells["A1:G1"].Merge = true;

                worksheet.Cells["A2:G2"].Value = $"Từ ngày {val.DateFrom ?? null} đến ngày {val.DateTo?? null}";
                worksheet.Cells["A2:G2"].Merge = true;

                worksheet.Cells[4, 1].Value = "Ngày tiếp nhận";
                worksheet.Cells[4, 2].Value = "Khách hàng";
                worksheet.Cells[4, 3].Value = "Dịch vụ";
                worksheet.Cells[4, 4].Value = "Bác sĩ";
                worksheet.Cells[4, 5].Value = "Giờ tiếp nhận";
                worksheet.Cells[4, 6].Value = "Thời gian phục vụ";
                worksheet.Cells[4, 6].Value = "Lý do không phục vụ";


                worksheet.Cells["A4:E4"].Style.Font.Bold = true;

                var row = 5;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.DateWaiting;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2].Value = item.Partner.Name;
                    worksheet.Cells[row, 3].Value = item.Products;
                    worksheet.Cells[row, 4].Value = item.DoctorName;
                    worksheet.Cells[row, 5].Value = item.DateWaiting;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "HH:mm";
                    worksheet.Cells[row, 6].Value = item.MinuteTotal;
                    worksheet.Cells[row, 7].Value = item.Reason;
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
