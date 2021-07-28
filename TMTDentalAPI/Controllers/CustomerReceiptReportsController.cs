using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCustomerReceiptForTime([FromQuery] CustomerReceiptReportFilter val)
        {
            var res = await _customerReceiptReportService.GetCustomerReceiptForTime(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCustomerReceiptForTimeDetail([FromQuery] CustomerReceiptTimeDetailFilter val)
        {
            var res = await _customerReceiptReportService.GetCustomerReceiptForTimeDetail(val);
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
        public async Task<IActionResult> ExportExcelReportOverview(CustomerReceiptReportFilter val)
        {
            val.Limit = int.MaxValue;
            var stream = new MemoryStream();
            var data = await _customerReceiptReportService.GetPagedResultAsync(val);
            byte[] fileContent;


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoTiepNhan");

                worksheet.Cells["A1:G1"].Value = "BÁO CÁO TỔNG QUAN TIẾP NHẬN";
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells["A1:G1"].Style.Font.Size = 20;
                worksheet.Cells["A1:G1"].Merge = true;

                worksheet.Cells["A2:G2"].Value = $"Từ ngày {val.DateFrom.Value.Date} đến ngày {val.DateTo.Value.Date}";
                worksheet.Cells["A2:G2"].Merge = true;

                worksheet.Cells[4, 1].Value = "Ngày tiếp nhận";
                worksheet.Cells[4, 2].Value = "Khách hàng";
                worksheet.Cells[4, 3].Value = "Dịch vụ";
                worksheet.Cells[4, 4].Value = "Bác sĩ";
                worksheet.Cells[4, 5].Value = "Giờ tiếp nhận";
                worksheet.Cells[4, 6].Value = "Thời gian phục vụ";
                worksheet.Cells[4, 7].Value = "Loại Khám";            
                worksheet.Cells[4, 8].Value = "Kết quả khám mới";
                worksheet.Cells[4, 9].Value = "Trạng thái";

                worksheet.Cells["A4:I4"].Style.Font.Bold = true;
                worksheet.Cells["A4:I4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                var row = 5;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.DateWaiting;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 2].Value = item.Partner.Name;
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 3].Value = item.Products;
                    worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 4].Value = item.DoctorName;
                    worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 5].Value = item.DateWaiting;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "HH:mm";
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 6].Value = item.MinuteTotal.HasValue ? $"{item.MinuteTotal} phút" : null;
                    worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 7].Value = item.IsRepeatCustomer == true ? "Tái khám" : "Khám mới";
                    worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 8].Value = item.IsRepeatCustomer == false && item.State == "done" ? (item.IsNoTreatment == true ? "Không điều trị" : "Có Điều trị" ) : null ;
                    worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 9].Value = item.State == "waiting" ? "Chờ khám" : (item.State == "examination" ? "Đang khám" : "Hoàn thành");
                    worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelReportForTime(CustomerReceiptReportFilter val)
        {
            val.Limit = int.MaxValue;
            var stream = new MemoryStream();
            var data = await _customerReceiptReportService.GetCustomerReceiptForTime(val);
            byte[] fileContent;


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoTiepNhan_TheoGioTiepNhan");

                worksheet.Cells["A1:G1"].Value = "BÁO CÁO TIẾP NHẬN THEO GIỜ TIẾP NHẬN  ";
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells["A1:G1"].Style.Font.Size = 20;
                worksheet.Cells["A1:G1"].Merge = true;

                worksheet.Cells["A2:G2"].Value = $"Từ ngày {val.DateFrom.Value.Date} đến ngày {val.DateTo.Value.Date}";
                worksheet.Cells["A2:G2"].Style.Numberformat.Format = "dd/mm/yyyy";
                worksheet.Cells["A2:G2"].Merge = true;

                var row = 5;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = $"Từ {item.Time}:00 đến {item.Time}:59";
                    worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.Blue);
                    worksheet.Cells[row, 10].Value = $"Tổng số lượng : {item.TimeRangeCount}";
                    var childs = await _customerReceiptReportService.GetCustomerReceiptForTimeDetail(new CustomerReceiptTimeDetailFilter
                    { 
                        Offset = val.Offset,
                        Limit = int.MaxValue,
                        CompanyId = val.CompanyId,
                        DateFrom = val.DateFrom,
                        DateTo = val.DateTo,
                        Time = item.Time
                    });

                    row ++;
                    if (childs.Items.Any())
                    {
                        worksheet.Cells[row, 2].Value = "Ngày tiếp nhận";
                        worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 3].Value = "Khách hàng";
                        worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 4].Value = "Dịch vụ";
                        worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 5].Value = "Bác sĩ";
                        worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 6].Value = "Giờ tiếp nhận";
                        worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 7].Value = "Thời gian phục vụ";
                        worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 8].Value = "Loại Khám";
                        worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 9].Value = "Kết quả khám mới";
                        worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 10].Value = "Trạng thái";
                        worksheet.Cells[row, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[$"B{row}:J{row}"].Style.Font.Bold = true;

                        row++;

                        foreach (var itemChild in childs.Items)
                        {
                            worksheet.Cells[row, 2].Value = itemChild.DateWaiting;
                            worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/mm/yyyy";
                            worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 3].Value = itemChild.Partner.Name;
                            worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 4].Value = itemChild.Products;
                            worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 5].Value = itemChild.DoctorName;
                            worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 6].Value = itemChild.DateWaiting;
                            worksheet.Cells[row, 6].Style.Numberformat.Format = "HH:mm";
                            worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 7].Value = itemChild.MinuteTotal.HasValue ? $"{itemChild.MinuteTotal} phút" : null;
                            worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 8].Value = itemChild.IsRepeatCustomer == true ? "Tái khám" : "Khám mới";
                            worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 9].Value = itemChild.IsRepeatCustomer == false && itemChild.State == "done" ? (itemChild.IsNoTreatment == true ? "Không điều trị" : "Có Điều trị") : null;
                            worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 10].Value = itemChild.State == "waiting" ? "Chờ khám" : (itemChild.State == "examination" ? "Đang khám" : "Hoàn thành");
                            worksheet.Cells[row, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            row++;
                        }


                    }
                    else
                    {
                        row++;
                    }

                }
                                           
                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelReportTimeService(CustomerReceiptReportFilter val)
        {
            val.Limit = int.MaxValue;
            var stream = new MemoryStream();
            var data = await _customerReceiptReportService.GetPagedResultAsync(val);
            byte[] fileContent;


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoTiepNhan_PhucVu");

                worksheet.Cells["A1:G1"].Value = "BÁO CÁO TIẾP NHẬN THEO THỜI GIAN PHỤC VỤ";
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells["A1:G1"].Style.Font.Size = 20;
                worksheet.Cells["A1:G1"].Merge = true;

                worksheet.Cells["A2:G2"].Value = $"Từ ngày {val.DateFrom.Value.Date} đến ngày {val.DateTo.Value.Date}";
                worksheet.Cells["A2:G2"].Merge = true;

                worksheet.Cells[4, 1].Value = "Ngày tiếp nhận";
                worksheet.Cells[4, 2].Value = "Khách hàng";
                worksheet.Cells[4, 3].Value = "Dịch vụ";
                worksheet.Cells[4, 4].Value = "Bác sĩ";
                worksheet.Cells[4, 5].Value = "Loại Khám";
                worksheet.Cells[4, 6].Value = "Thời gian phục vụ";
                worksheet.Cells[4, 7].Value = "Thời gian chờ khám";
                worksheet.Cells[4, 8].Value = "Thời gian khám";

                worksheet.Cells["A4:H4"].Style.Font.Bold = true;
                worksheet.Cells["A4:H4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                var row = 5;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.DateWaiting;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 2].Value = item.Partner.Name;
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 3].Value = item.Products;
                    worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 4].Value = item.DoctorName;
                    worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 5].Value = item.IsRepeatCustomer == true ? "Tái khám" : "Khám mới";
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 6].Value = item.MinuteTotal.HasValue ? $"{item.MinuteTotal} phút" : null;
                    worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 7].Value = item.MinuteWaiting.HasValue ? $"{item.MinuteWaiting} phút" : null;
                    worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 8].Value = item.MinuteExamination.HasValue ? $"{item.MinuteExamination} phút" : null;
                    worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
                worksheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells["A1:G1"].Style.Font.Size = 20;
                worksheet.Cells["A1:G1"].Merge = true;

                worksheet.Cells["A2:G2"].Value = $"Từ ngày {val.DateFrom.Value.Date} đến ngày {val.DateTo.Value.Date}";
                worksheet.Cells["A2:G2"].Merge = true;

                worksheet.Cells[4, 1].Value = "Ngày tiếp nhận";
                worksheet.Cells[4, 2].Value = "Khách hàng";
                worksheet.Cells[4, 3].Value = "Dịch vụ";
                worksheet.Cells[4, 4].Value = "Bác sĩ";
                worksheet.Cells[4, 5].Value = "Giờ tiếp nhận";
                worksheet.Cells[4, 6].Value = "Thời gian phục vụ";
                worksheet.Cells[4, 7].Value = "Lý do không phục vụ";

                worksheet.Cells["A4:G4"].Style.Font.Bold = true;
                worksheet.Cells["A4:G4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                var row = 5;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.DateWaiting;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 2].Value = item.Partner.Name;
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 3].Value = item.Products;
                    worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 4].Value = item.DoctorName;
                    worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 5].Value = item.DateWaiting;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "HH:mm";
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 6].Value = $"{item.MinuteTotal} phút";
                    worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 7].Value = item.Reason;
                    worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
