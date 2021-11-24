using AutoMapper;
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
using ApplicationCore.Utilities;
using Microsoft.EntityFrameworkCore;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardReportsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IDashboardReportService _dashboardService;
        private readonly IAccountMoveLineService _amlService;
        private readonly ISaleOrderService _saleOrderService;
        private readonly ISaleOrderLineService _saleOrderLineService;
        private readonly ISaleReportService _saleReportService;
        private readonly ICashBookService _cashBookService;

        public DashboardReportsController(IMapper mapper, IDashboardReportService dashboardService,
            IAccountMoveLineService amlService,
            ISaleOrderService saleOrderService,
            ISaleOrderLineService saleOrderLineService,
            ISaleReportService saleReportService,
            ICashBookService cashBookService)
        {
            _mapper = mapper;
            _dashboardService = dashboardService;
            _amlService = amlService;
            _saleOrderService = saleOrderService;
            _saleOrderLineService = saleOrderLineService;
            _saleReportService = saleReportService;
            _cashBookService = cashBookService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDefaultCustomerReceipt(GetDefaultRequest val)
        {
            var res = await _dashboardService.GetDefaultCustomerReceipt(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCountMedicalXamination(ReportTodayRequest val)
        {
            var result = await _dashboardService.GetCountMedicalXaminationToday(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCustomerReceiptToAppointment(CustomerReceiptRequest val)
        {
            var res = await _dashboardService.CreateCustomerReceiptToAppointment(val);
            return Ok(res);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetSumary(ReportTodayRequest val)
        {
            var res = await _dashboardService.GetSumary(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetRevenueChartReport(ReportRevenueChartFilter val)
        {
            var res = await _dashboardService.GetRevenueChartReport(val.DateFrom, val.DateTo, val.CompanyId, val.GroupBy);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSumaryRevenueReport(SumaryRevenueReportFilter val)
        {
            var res = await _dashboardService.GetSumaryRevenueReport(val.DateFrom, val.DateTo, val.CompanyId, val.AccountCode, val.ResultSelection);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetRevenueActualReport(GetRevenueActualReportRequest val)
        {
            //báo cáo doanh thu thực thu
            var dateFrom = val.DateFrom.HasValue ? val.DateFrom.Value.AbsoluteBeginOfDate() : (DateTime?)null;
            var dateTo = val.DateTo.HasValue ? val.DateTo.Value.AbsoluteEndOfDate() : (DateTime?)null;
            var query = _amlService._QueryGet(dateTo: dateTo, dateFrom: dateFrom, state: "posted", companyId: val.CompanyId);
            var cashBankPaymentTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.AccountInternalType == "receivable").SumAsync(x => x.Credit);
            var advancePaymentTotal = await query.Where(x => x.Journal.Type == "advance" && x.AccountInternalType == "receivable").SumAsync(x => x.Credit);
            var debtPaymentTotal = await query.Where(x => x.Journal.Type == "debt" && x.AccountInternalType == "receivable").SumAsync(x => x.Credit);
            var debtInsuranceTotal = await query.Where(x => x.Journal.Type == "insurance" && x.AccountInternalType == "receivable").SumAsync(x => x.Credit);

            var advanceIncomeTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.Account.Code == "KHTU").SumAsync(x => x.Credit);
            var debtIncomeTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.Account.Code == "CNKH").SumAsync(x => x.Credit);
            var cashBankDebitTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.AccountInternalType == "liquidity").SumAsync(x => x.Debit);
            var insuranceIncomeTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.Account.Code == "CNBH").SumAsync(x => x.Credit);

            return Ok(new GetRevenueActualReportResponse
            {
                CashBankPaymentTotal = cashBankPaymentTotal,
                AdvancePaymentTotal = advancePaymentTotal,
                DebtPaymentTotal = debtPaymentTotal,
                DebtInsuranceTotal = debtInsuranceTotal,
                InsuranceIncomeTotal = insuranceIncomeTotal,
                AdvanceIncomeTotal = advanceIncomeTotal,
                DebtIncomeTotal = debtIncomeTotal,
                CashBankDebitTotal = cashBankDebitTotal
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetThuChiReport(GetRevenueActualReportRequest val)
        {
            //báo cáo doanh thu thực thu
            var res = await _dashboardService.GetThuChiReport(val.DateFrom, val.DateTo, val.CompanyId, val.JournalId);         
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSummaryReport(GetSummaryReportRequest val)
        {
            var dateFrom = val.DateFrom.HasValue ? val.DateFrom.Value.AbsoluteBeginOfDate() : (DateTime?)null;
            var dateTo = val.DateTo.HasValue ? val.DateTo.Value.AbsoluteEndOfDate() : (DateTime?)null;
            var query = _amlService._QueryGet(dateTo: dateTo, dateFrom: dateFrom, state: "posted", companyId: val.CompanyId);

            var cashTotal = await query.Where(x => x.Journal.Type == "cash" && x.AccountInternalType == "liquidity").SumAsync(x => x.Debit - x.Credit);
            var bankTotal = await query.Where(x => x.Journal.Type == "bank" && x.AccountInternalType == "liquidity").SumAsync(x => x.Debit - x.Credit);
            var payableTotal = await query.Where(x => x.AccountInternalType == "payable").SumAsync(x => x.Credit - x.Debit);
            var debtTotal = await query.Where(x => x.Account.Code == "CNKH").SumAsync(x => x.Debit - x.Credit);
            var expectTotal = await _saleOrderService.SearchQuery(x => (!val.CompanyId.HasValue || x.CompanyId == val.CompanyId) && x.State != "draft").SumAsync(x => (x.AmountTotal ?? 0) - (x.TotalPaid ?? 0));

            return Ok(new GetSummaryReportResponse
            {
                CashTotal = cashTotal,
                BankTotal = bankTotal,
                DebtTotal = debtTotal,
                PayableTotal = payableTotal,
                ExpectTotal = expectTotal
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelDayReport(ExportExcelDashBoardDayFilter val)
        {
            var stream = new MemoryStream();
            byte[] fileContent;


            using (var package = new ExcelPackage(stream))
            {

                ///tab dịch vụ trong ngày
                var worksheet1 = package.Workbook.Worksheets.Add("DichVuDangKiMoi");
                var data = await _saleOrderLineService.GetPagedResultAsync(new SaleOrderLinesPaged {CompanyId = val.CompanyId, DateFrom = val.DateFrom , DateTo = val.DateTo , State = "sale,done,cancel" });

                worksheet1.Cells["A1:I1"].Value = "DỊCH VỤ ĐĂNG KÝ MỚI";
                worksheet1.Cells["A1:I1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet1.Cells["A1:I1"].Style.Font.Size = 16;
                worksheet1.Cells["A1:I1"].Merge = true;
                worksheet1.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //worksheet1.Cells["A1:I1"].Style.Font.Bold = true;
                worksheet1.Cells["A1:I1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));

                worksheet1.Cells["A2:I2"].Value = @$"Ngày {val.DateFrom.Value.ToShortDateString()}";
                worksheet1.Cells["A2:I2"].Merge = true;
                worksheet1.Cells["A2:I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheet1.Cells[4, 1, 4, 2].Value = "Dịch vụ";
                worksheet1.Cells[4, 3, 4, 4].Value = "Số khách hàng";
                worksheet1.Cells[4, 5, 4, 7].Value = "Tổng tiền điều trị";
                worksheet1.Cells[4, 8, 4, 9].Value = "Thanh toàn";

                worksheet1.Cells["A5:B5"].Value = data.Items.Count();
                worksheet1.Cells["C5:D5"].Value = data.Items.Select(x => x.OrderPartnerId).Distinct().Count();
                worksheet1.Cells["E5:G5"].Value = data.Items.Sum(x => x.PriceSubTotal);
                worksheet1.Cells["H5:I5"].Value = data.Items.Sum(x => x.PriceSubTotal) - data.Items.Sum(x => x.AmountResidual);

                worksheet1.Cells["E5:I5"].Style.Numberformat.Format = "#,##0";
                worksheet1.Cells[5, 1, 5, 2].Merge = true;
                worksheet1.Cells[4, 1, 4, 2].Merge = true;

                worksheet1.Cells[4, 3, 4, 4].Merge = true;
                worksheet1.Cells[5, 3, 5, 4].Merge = true;

                worksheet1.Cells[4, 5, 4, 7].Merge = true;
                worksheet1.Cells[5, 5, 5, 7].Merge = true;

                worksheet1.Cells[4, 8, 4, 9].Merge = true;
                worksheet1.Cells[5, 8, 5, 9].Merge = true;

                worksheet1.Cells["A4:I4"].Style.Font.Bold = true;
                worksheet1.Cells["A4:I4"].Style.Font.Size = 11;
                worksheet1.Cells["A4:I4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet1.Cells["A4:I4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet1.Cells["A4:I4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet1.Cells["A4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0667d1"));
                worksheet1.Cells["A4:I4"].Style.Font.Color.SetColor(Color.White);

                //worksheet1.Cells["A5:I5"].Style.Font.Bold = true;
                worksheet1.Cells["A5:I5"].Style.Font.Size = 14;
                worksheet1.Cells["A5:I5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet1.Cells["A5:I5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet1.Cells["A5:I5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet1.Cells["A5:I5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0667d1"));
                worksheet1.Cells["A5:I5"].Style.Font.Color.SetColor(Color.White);

                worksheet1.Cells[7, 1].Value = "Dịch vụ";
                worksheet1.Cells[7, 2].Value = "Phiếu điều trị";
                worksheet1.Cells[7, 3].Value = "Khách hàng";
                worksheet1.Cells[7, 4].Value = "Số lượng";
                worksheet1.Cells[7, 5].Value = "Bác sĩ";
                worksheet1.Cells[7, 6].Value = "Thành tiền";
                worksheet1.Cells[7, 7].Value = "Thanh toán";
                worksheet1.Cells[7, 8].Value = "Còn lại";
                worksheet1.Cells[7, 9].Value = "Trạng thái";

                worksheet1.Cells["A7:I7"].Style.Font.Bold = true;
                worksheet1.Cells["A7:I7"].Style.Font.Size = 11;
                //worksheet1.Cells["A7:I7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet1.Cells["A7:I7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet1.Cells["A7:I7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet1.Cells["A7:I7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet1.Cells["A7:I7"].Style.Border.Bottom.Color.SetColor(Color.White);
                worksheet1.Cells["A7:I7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet1.Cells["A7:I7"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0667d1"));
                worksheet1.Cells["A7:I7"].Style.Font.Color.SetColor(Color.White);

                var row = 8;
                foreach (var item in data.Items)
                {
                    worksheet1.Cells[row, 1].Value = item.Name;
                    worksheet1.Cells[row, 2].Value = item.Order.Name;
                    worksheet1.Cells[row, 3].Value = item.OrderPartner.Name;
                    worksheet1.Cells[row, 4].Value = item.ProductUOMQty;
                    worksheet1.Cells[row, 5].Value = item.Employee != null ? item.Employee.Name : null;
                    worksheet1.Cells[row, 6].Value = item.PriceSubTotal;
                    worksheet1.Cells[row, 7].Value = (item.AmountInvoiced ?? 0);
                    worksheet1.Cells[row, 8].Value = (item.AmountResidual ?? 0);
                    worksheet1.Cells[row, 9].Value = item.State == "sale" ? "Đang điều trị" : (item.State == "done" ? "Hoàn thành" : "Ngừng điều trị");

                    //worksheet1.Cells[row, 1, row, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet1.Cells[row, 1, row, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet1.Cells[row, 1, row, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet1.Cells[row, 1, row, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet1.Cells[row, 1, row, 9].Style.Border.Bottom.Color.SetColor(Color.White);
                    worksheet1.Cells[row, 1, row, 9].Style.Font.Size = 11;
                    worksheet1.Cells[row, 6, row, 8].Style.Numberformat.Format = "#,##0";
                    row++;
                }

                ///tab doanh thu
                var worksheet2 = package.Workbook.Worksheets.Add("DoanhThu");
                var dataRevenue = await _cashBookService.GetDataInvoices(val.DateFrom, val.DateTo, val.CompanyId, "all");
                var query = _amlService._QueryGet(dateTo: val.DateTo, dateFrom: val.DateFrom, state: "posted", companyId: val.CompanyId);
                var cashBankPaymentTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.AccountInternalType == "receivable").SumAsync(x => x.Credit);
                var advancePaymentTotal = await query.Where(x => x.Journal.Type == "advance" && x.AccountInternalType == "receivable").SumAsync(x => x.Credit);
                var debtPaymentTotal = await query.Where(x => x.Journal.Type == "debt" && x.AccountInternalType == "receivable").SumAsync(x => x.Credit);
                var debtInsuranceTotal = await query.Where(x => x.Journal.Type == "insurance" && x.AccountInternalType == "receivable").SumAsync(x => x.Credit);

                worksheet2.Cells["A1:E1"].Value = "DOANH THU";
                worksheet2.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet2.Cells["A1:E1"].Style.Font.Size = 16;
                worksheet2.Cells["A1:E1"].Merge = true;
                worksheet2.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //worksheet2.Cells["A1:E1"].Style.Font.Bold = true;
                worksheet2.Cells["A1:E1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
               
                worksheet2.Cells["A2:E2"].Value = @$"Ngày {val.DateFrom.Value.ToShortDateString()}";
                worksheet2.Cells["A2:E2"].Merge = true;
                worksheet2.Cells["A2:E2"].Style.Font.Size = 11;
                worksheet2.Cells["A2:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet2.Cells["A4:D4"].Value = "Doanh thu";
                worksheet2.Cells["A4:D4"].Merge = true;

                worksheet2.Cells["A5:C5"].Value = "Thực thu bằng tiền mặt/ ngân hàng";
                worksheet2.Cells["A5:C5"].Merge = true;
                worksheet2.Cells["D5"].Value = cashBankPaymentTotal;

                worksheet2.Cells["A6:C6"].Value = "Thanh toán bằng tạm ứng";
                worksheet2.Cells["A6:C6"].Merge = true;
                worksheet2.Cells["D6"].Value = advancePaymentTotal;

                worksheet2.Cells["A7:C7"].Value = "Thanh toán bằng ghi công nợ";
                worksheet2.Cells["A7:C7"].Merge = true;
                worksheet2.Cells["D7"].Value = debtPaymentTotal;

                worksheet2.Cells["A8:C8"].Value = "Thanh toán bằng bảo hiểm";
                worksheet2.Cells["A8:C8"].Merge = true;
                worksheet2.Cells["D8"].Value = debtInsuranceTotal;

                worksheet2.Cells["A9:C9"].Value = "Tổng";
                worksheet2.Cells["A9:C9"].Merge = true;
                worksheet2.Cells["D9"].Value = cashBankPaymentTotal + advancePaymentTotal + debtPaymentTotal + debtInsuranceTotal;

                worksheet2.Cells[4, 1, 9, 4].Style.Font.Bold = true;
                worksheet2.Cells[4, 1, 9, 4].Style.Font.Size = 11;
                worksheet2.Cells[4, 1, 9, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet2.Cells[4, 1, 9, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet2.Cells[4, 1, 9, 4].Style.Border.Left.Color.SetColor(Color.White);
                worksheet2.Cells[4, 1, 9, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet2.Cells[4, 1, 8, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet2.Cells[4, 1, 8, 4].Style.Border.Bottom.Color.SetColor(Color.White);
                worksheet2.Cells[9, 1, 9, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;             
                worksheet2.Cells["D5:D9"].Style.Numberformat.Format = "#,##0";

                worksheet2.Cells["A4:D4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet2.Cells["A4:D4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0667d1"));
                worksheet2.Cells["A4:D4"].Style.Font.Color.SetColor(Color.White);




                worksheet2.Cells[11, 1].Value = "Mã thanh toán";
                worksheet2.Cells[11, 2].Value = "Nội dung";
                worksheet2.Cells[11, 3].Value = "Khách hàng";
                worksheet2.Cells[11, 4].Value = "Phương thức";
                worksheet2.Cells[11, 5].Value = "Số tiền";

                worksheet2.Cells["A11:E11"].Style.Font.Bold = true;
                worksheet2.Cells["A11:E11"].Style.Font.Size = 11;
                worksheet2.Cells["A11:E11"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet2.Cells["A11:E11"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet2.Cells["A11:E11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet2.Cells["A11:E11"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet1.Cells["A11:E11"].Style.Border.Bottom.Color.SetColor(Color.White);
                worksheet2.Cells["A11:E11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet2.Cells["A11:E11"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0667d1"));
                worksheet2.Cells["A11:E11"].Style.Font.Color.SetColor(Color.White);

                var row2 = 12;
                foreach (var item in dataRevenue)
                {
                    worksheet2.Cells[row2, 1].Value = item.InvoiceOrigin;
                    worksheet2.Cells[row2, 2].Value = item.Name;
                    worksheet2.Cells[row2, 3].Value = item.PartnerName;
                    worksheet2.Cells[row2, 4].Value = item.JournalName;
                    worksheet2.Cells[row2, 5].Value = item.Amount;
                    worksheet2.Cells[row2, 5].Style.Numberformat.Format = "#,##0";

                    worksheet2.Cells[row2, 1, row2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[row2, 1, row2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[row2, 1, row2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet2.Cells[row2, 1, row2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //worksheet1.Cells[row2, 1, row2, 5].Style.Border.Bottom.Color.SetColor(Color.White);
                    row2++;
                }

                ///Thu chi
                var worksheet3 = package.Workbook.Worksheets.Add("SoQuy");
                var dataCashBook = await _dashboardService.GetDataCashBookReportDay(val.DateFrom, val.DateTo, val.CompanyId);

                worksheet3.Cells["A1:F1"].Value = "SỔ QUỸ";
                worksheet3.Cells["A1:F1"].Style.Font.Color.SetColor(Color.Blue);
                worksheet3.Cells["A1:F1"].Style.Font.Size = 14;
                worksheet3.Cells["A1:F1"].Merge = true;
                worksheet3.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet3.Cells["A1:F1"].Style.Font.Bold = true;
                worksheet3.Cells["A1:F1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet3.Cells["A2:F2"].Value = @$"Ngày {val.DateFrom.Value.ToShortDateString()}";

                worksheet3.Cells["A2:F2"].Merge = true;
                worksheet3.Cells["A2:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

              
                worksheet3.Cells["A4:B4"].Value = "Tồn quỹ";
                worksheet3.Cells["A4:B4"].Merge = true;
                worksheet3.Cells["A4:B4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                worksheet3.Cells["C4:D4"].Value = "Các khoản thu";
                worksheet3.Cells["C4:D4"].Merge = true;
                worksheet3.Cells["C4:D4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                worksheet3.Cells["E4:F4"].Value = "Các khoản chi";
                worksheet3.Cells["E4:F4"].Merge = true;
                worksheet3.Cells["E4:F4"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                worksheet3.Cells["A4:F4"].Style.Font.Bold = true;
                worksheet3.Cells["A4:F4"].Style.Font.Size = 11;
                worksheet3.Cells["A4:F4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet3.Cells["A4:F4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0667d1"));
                worksheet3.Cells["A4:F4"].Style.Font.Color.SetColor(Color.White);

                worksheet3.Cells["A5"].Value = "Đầu ngày";
                worksheet3.Cells["B5"].Value = dataCashBook.SumaryDayReport.Begin;
                worksheet3.Cells["B5"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["A6"].Value = "Tổng thu trong ngày";
                worksheet3.Cells["B6"].Value = dataCashBook.SumaryDayReport.TotalThu;
                worksheet3.Cells["B6"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["A7"].Value = "Tổng chi trong ngày";
                worksheet3.Cells["B7"].Value = dataCashBook.SumaryDayReport.TotalChi;
                worksheet3.Cells["B7"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["A8"].Value = "Chênh lệch trong ngày";
                worksheet3.Cells["B8"].Value = dataCashBook.SumaryDayReport.TotalThu - dataCashBook.SumaryDayReport.TotalChi;
                worksheet3.Cells["B8"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["A9"].Value = "Cuối ngày";
                worksheet3.Cells["B9"].Value = dataCashBook.SumaryDayReport.TotalAmount;
                worksheet3.Cells["B9"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["C5"].Value = "Khách hàng thanh toán dịch vụ và thuốc";
                worksheet3.Cells["D5"].Value = dataCashBook.DataThuChiReport.CustomerIncomeTotal;
                worksheet3.Cells["D5"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["C6"].Value = "Khách hàng đóng tạm ứng";
                worksheet3.Cells["D6"].Value = dataCashBook.DataThuChiReport.AdvanceIncomeTotal;
                worksheet3.Cells["D6"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["C7"].Value = "Thu công nợ khách hàng";
                worksheet3.Cells["D7"].Value = dataCashBook.DataThuChiReport.DebtIncomeTotal;
                worksheet3.Cells["D7"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["C8"].Value = "Thu công nợ bảo hiểm";
                worksheet3.Cells["D8"].Value = dataCashBook.DataThuChiReport.InsuranceIncomeTotal;
                worksheet3.Cells["D8"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["C9"].Value = "Nhà cung cấp hoàn tiền";
                worksheet3.Cells["D9"].Value = dataCashBook.DataThuChiReport.SupplierIncomeTotal;
                worksheet3.Cells["D9"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["C10"].Value = "Thu ngoài";
                worksheet3.Cells["D10"].Value = dataCashBook.DataThuChiReport.OtherIncomeTotal;
                worksheet3.Cells["D10"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["E5"].Value = "Thanh toán cho nhà cung cấp";
                worksheet3.Cells["F5"].Value = dataCashBook.DataThuChiReport.SupplierExpenseTotal;
                worksheet3.Cells["F5"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["E6"].Value = "Hoàn tạm ứng cho khách hàng";
                worksheet3.Cells["F6"].Value = dataCashBook.DataThuChiReport.AdvanceExpenseTotal;
                worksheet3.Cells["F6"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["E7"].Value = "Chi lương nhân viên";
                worksheet3.Cells["F7"].Value = dataCashBook.DataThuChiReport.SalaryExpenseTotal;
                worksheet3.Cells["F7"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["E8"].Value = "Hoa hồng người giới thiệu";
                worksheet3.Cells["F8"].Value = dataCashBook.DataThuChiReport.CommissionExpenseTotal;
                worksheet3.Cells["F8"].Style.Numberformat.Format = "#,##0";

                worksheet3.Cells["E9"].Value = "Chi ngoài";
                worksheet3.Cells["F9"].Value = dataCashBook.DataThuChiReport.OtherExpenseTotal;
                worksheet3.Cells["F9"].Style.Numberformat.Format = "#,##0";

                //worksheet3.Cells[5, 1, 10, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells[5, 1, 10, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells[5, 1, 10, 6].Style.Border.Left.Color.SetColor(Color.White);
                worksheet3.Cells["B5:B10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells["D5:D10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells["F5:F10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells[5, 1, 9, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells[5, 1, 9, 6].Style.Border.Bottom.Color.SetColor(Color.White);
                worksheet3.Cells[10, 1, 10, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells[5, 1, 10, 6].Style.Font.Bold = true;
                worksheet3.Cells[5, 1, 10, 6].Style.Font.Size = 11;
           


                worksheet3.Cells[12, 1].Value = "Số phiếu";
                worksheet3.Cells[12, 2].Value = "Nội dung";
                worksheet3.Cells[12, 3].Value = "Phương thức";
                worksheet3.Cells[12, 4].Value = "Loại thu chi";
                worksheet3.Cells[12, 5].Value = "Số tiền";
                worksheet3.Cells[12, 6].Value = "Đối tác";

                worksheet3.Cells["A12:F12"].Style.Font.Bold = true;
                worksheet3.Cells["A12:F12"].Style.Font.Size = 11;
                worksheet3.Cells["A12:F12"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells["A12:F12"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells["A12:F12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells["A12:F12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet3.Cells["A12:F12"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet3.Cells["A12:F12"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0667d1"));
                worksheet3.Cells["A12:F12"].Style.Font.Color.SetColor(Color.White);

                var row3 = 13;
                foreach (var item in dataCashBook.DataDetails)
                {
                    worksheet3.Cells[row3, 1].Value = item.InvoiceOrigin;
                    worksheet3.Cells[row3, 2].Value = item.Name;
                    worksheet3.Cells[row3, 3].Value = item.JournalName;
                    worksheet3.Cells[row3, 4].Value = item.AccountName;
                    worksheet3.Cells[row3, 5].Value = item.Amount;
                    worksheet3.Cells[row3, 5].Style.Numberformat.Format = "#,##0";
                    worksheet3.Cells[row3, 6].Value = item.PartnerName;

                    worksheet3.Cells[row3, 1, row3, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet3.Cells[row3, 1, row3, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet3.Cells[row3, 1, row3, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet3.Cells[row3, 1, row3, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    row3++;
                }




                worksheet1.Cells.AutoFitColumns();
                worksheet2.Cells.AutoFitColumns();
                worksheet3.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }
    }
}
