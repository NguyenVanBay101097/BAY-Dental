using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class GetDefaultRequest
    {
        public Guid? AppointmentId { get; set; }
    }


    public class GetCountMedicalXamination
    {
        public int NewMedical { get; set; }
        public int ReMedical { get; set; }
    }

    public class CustomerReceiptRequest
    {
        /// <summary>
        /// thời gian chờ khám
        /// </summary>
        public DateTime? DateWaiting { get; set; }

        /// <summary>
        /// Thời gian dự kiến
        /// </summary>
        public int TimeExpected { get; set; }

        /// <summary>
        /// Danh sách dịch vụ
        /// </summary>
        public IEnumerable<ProductSimple> Products { get; set; } = new List<ProductSimple>();

        /// <summary>
        /// Ghi chú, nội dung
        /// </summary>
        public string Note { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? DoctorId { get; set; }

        /// <summary>
        /// Khách hàng tái khám
        /// </summary>
        public bool IsRepeatCustomer { get; set; }

        public Guid? AppointmentId { get; set; }
    }

    public class CustomerReceiptReqonse
    {
        public CustomerReceiptBasic CustomerReceipt { get; set; }
        public AppointmentBasic Appointment { get; set; }
    }

    public class ReportTodayRequest
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class RevenueTodayReponse
    {
        public RevenueTodayReponse()
        {
            TotalAmountYesterday = 0;
            TotalOther = 0;
            TotalBank = 0;
            TotalCash = 0;
            TotalAmount = 0;
        }
      
        public decimal TotalBank { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalOther{ get; set; }
        public decimal TotalAmountYesterday { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SumaryRevenueReport
    {
        public string Type { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Balance { get; set; }
    }

    public class SumaryRevenueReportFilter
    {
        /// <summary>
        /// Ngay bat dau
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// ngay ket thuc
        /// </summary>
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// debt: công nợ
        /// advance: tạm ứng
        /// cash_bank: TM/NH
        /// </summary>
        public string ResultSelection { get; set; }

        public string AccountCode { get; set; }
    }

    public class ReportRevenueChart
    {
        public DateTime Date { get; set; }

        public decimal AmountRevenue { get; set; }

        public decimal AmountCashBook { get; set; }
    }

    public class RevenueReportChartVM
    {
        public DateTime Date { get; set; }

        public decimal TotalSaleAmount { get; set; }

        public decimal TotalRevenueAmount { get; set; }

        public decimal TotalLiquidityAmount { get; set; }
    }

    public class ReportRevenueChartFilter
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }      

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// groupby:day  group theo ngày
        /// groupby:month: group theo tháng
        /// groupby:product: group theo dịch vụ
        /// groupby:employee: group theo nhân viên
        /// groupby:assistant: group theo phụ tá
        /// </summary>
        public string GroupBy { get; set; }
    }

    public class GetRevenueActualReportRequest
    {
        public DateTime? DateTo { get; set; }

        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        public Guid? JournalId { get; set; }
    }

    public class GetRevenueActualReportResponse
    {
        public decimal RevenuePaymentTotal
        {
            get
            {
                return CashBankPaymentTotal + AdvancePaymentTotal + DebtPaymentTotal + DebtInsuranceTotal;
            }
        }

        public decimal CashBankPaymentTotal { get; set; }

        public decimal AdvancePaymentTotal { get; set; }

        public decimal DebtPaymentTotal { get; set; }

        public decimal CashBankDebitTotal { get; set; }

        public decimal AdvanceIncomeTotal { get; set; }

        public decimal DebtIncomeTotal { get; set; }

        public decimal DebtInsuranceTotal { get; set; }

        public decimal InsuranceIncomeTotal { get; set; }

        public decimal OtherIncomeTotal 
        {
            get
            {
                return CashBankDebitTotal - CashBankPaymentTotal - AdvanceIncomeTotal - DebtIncomeTotal - InsuranceIncomeTotal;
            }
        }
    }

    public class GetThuChiReportRequest
    {
        public DateTime? DateTo { get; set; }

        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class GetThuChiReportResponse
    {
        public decimal CustomerIncomeTotal { get; set; }

        public decimal AdvanceIncomeTotal { get; set; }

        public decimal DebtIncomeTotal { get; set; }

        public decimal SupplierIncomeTotal { get; set; }

        public decimal CashBankIncomeTotal { get; set; }

        public decimal InsuranceIncomeTotal { get; set; }

        public decimal OtherIncomeTotal
        {
            get
            {
                return CashBankIncomeTotal - CustomerIncomeTotal - AdvanceIncomeTotal - DebtIncomeTotal - SupplierIncomeTotal - InsuranceIncomeTotal;
            }
        }

        public decimal SupplierExpenseTotal { get; set; }

        public decimal AdvanceExpenseTotal { get; set; }

        public decimal SalaryExpenseTotal { get; set; }

        public decimal CommissionExpenseTotal { get; set; }

        public decimal CashBankExpenseTotal { get; set; }

        public decimal OtherExpenseTotal
        {
            get
            {
                return CashBankExpenseTotal - SupplierExpenseTotal - AdvanceExpenseTotal - SalaryExpenseTotal - CommissionExpenseTotal;
            }
        }
    }

    public class GetSummaryReportRequest
    {
        public DateTime? DateTo { get; set; }

        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class GetSummaryReportResponse
    {
        public decimal CashTotal { get; set; }

        public decimal BankTotal { get; set; }

        public decimal PayableTotal { get; set; }

        public decimal DebtTotal { get; set; }

        public decimal ExpectTotal { get; set; }
        public decimal InsuranceDebitTotal { get; set; }

    }
    public class ExportExcelDashBoardDayFilter
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }

}
