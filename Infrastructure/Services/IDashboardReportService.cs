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

        Task<SumaryRevenueReport> GetSumaryRevenueReport(SumaryRevenueReportFilter val);

        Task<IEnumerable<ReportRevenueChart>> GetRevenueChartReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string groupBy);
        
    }
}
