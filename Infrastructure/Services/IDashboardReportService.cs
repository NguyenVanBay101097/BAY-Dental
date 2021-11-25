using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IDashboardReportService
    {
        Task<CustomerReceiptDisplay> GetDefaultCustomerReceipt(GetDefaultRequest val);
        Task<CustomerReceiptReqonse> CreateCustomerReceiptToAppointment(CustomerReceiptRequest val);

        Task<GetCountMedicalXamination> GetCountMedicalXaminationToday(ReportTodayRequest val);

        Task<RevenueTodayReponse> GetSumary(ReportTodayRequest val);

        Task<IEnumerable<ReportRevenueChart>> GetRevenueChartReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string groupBy);
        
        Task<SumaryRevenueReport> GetSumaryRevenueReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string accountCode, string resultSelection);

        Task<CashBookReportDay> GetDataCashBookReportDay(DateTime? dateFrom, DateTime? dateTo, Guid? companyId);

        Task<GetThuChiReportResponse> GetThuChiReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, Guid? journalId);
    }
}
