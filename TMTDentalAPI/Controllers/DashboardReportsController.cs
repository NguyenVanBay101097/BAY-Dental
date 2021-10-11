using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        public DashboardReportsController(IMapper mapper, IDashboardReportService dashboardService,
            IAccountMoveLineService amlService,
            ISaleOrderService saleOrderService)
        {
            _mapper = mapper;
            _dashboardService = dashboardService;
            _amlService = amlService;
            _saleOrderService = saleOrderService;
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
            var res = await _dashboardService.GetSumaryRevenueReport(val);
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

            var advanceIncomeTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.Account.Code == "KHTU").SumAsync(x => x.Credit);
            var debtIncomeTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.Account.Code == "CNKH").SumAsync(x => x.Credit);
            var cashBankDebitTotal = await query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.AccountInternalType == "liquidity").SumAsync(x => x.Debit);

            return Ok(new GetRevenueActualReportResponse
            {
                CashBankPaymentTotal = cashBankPaymentTotal,
                AdvancePaymentTotal = advancePaymentTotal,
                DebtPaymentTotal = debtPaymentTotal,
                AdvanceIncomeTotal = advanceIncomeTotal,
                DebtIncomeTotal = debtIncomeTotal,
                CashBankDebitTotal = cashBankDebitTotal
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetThuChiReport(GetRevenueActualReportRequest val)
        {
            //báo cáo doanh thu thực thu
            var dateFrom = val.DateFrom.HasValue ? val.DateFrom.Value.AbsoluteBeginOfDate() : (DateTime?)null;
            var dateTo = val.DateTo.HasValue ? val.DateTo.Value.AbsoluteEndOfDate() : (DateTime?)null;
            var query = _amlService._QueryGet(dateTo: dateTo, dateFrom: dateFrom, state: "posted", companyId: val.CompanyId);
            query = query.Where(x => (x.Journal.Type == "cash" || x.Journal.Type == "bank") && x.AccountInternalType != "liquidity");

            var customerIncomeTotal = await query.Where(x => x.AccountInternalType == "receivable").SumAsync(x => x.Credit);
            var advanceIncomeTotal = await query.Where(x => x.Account.Code == "KHTU").SumAsync(x => x.Credit);
            var debtIncomeTotal = await query.Where(x => x.Account.Code == "CNKH").SumAsync(x => x.Credit);
            var supplierIncomeTotal = await query.Where(x => x.AccountInternalType == "payable").SumAsync(x => x.Credit);
            var cashBankIncomeTotal = await query.SumAsync(x => x.Credit);

            var supplierExpenseTotal = await query.Where(x => x.AccountInternalType == "payable").SumAsync(x => x.Debit);
            var advanceExpenseTotal = await query.Where(x => x.Account.Code == "KHTU").SumAsync(x => x.Debit);
            var salaryExpenseTotal = await query.Where(x => x.Account.Code == "334").SumAsync(x => x.Debit);
            var commissionExpenseTotal = await query.Where(x => x.Account.Code == "HHNGT").SumAsync(x => x.Debit);
            var cashBankExpenseTotal = await query.SumAsync(x => x.Debit);

            return Ok(new GetThuChiReportResponse
            {
                CustomerIncomeTotal = customerIncomeTotal,
                AdvanceIncomeTotal = advanceIncomeTotal,
                DebtIncomeTotal = debtIncomeTotal,
                SupplierIncomeTotal = supplierIncomeTotal,
                CashBankIncomeTotal = cashBankIncomeTotal,
                SupplierExpenseTotal = supplierExpenseTotal,
                AdvanceExpenseTotal = advanceExpenseTotal,
                SalaryExpenseTotal = salaryExpenseTotal,
                CommissionExpenseTotal = commissionExpenseTotal,
                CashBankExpenseTotal = cashBankExpenseTotal
            });
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
    }
}
