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

        Task<long> GetCountSaleOrder(ReportTodayRequest val);
        Task<RevenueTodayReponse> GetSumary(ReportTodayRequest val);
    }
}
