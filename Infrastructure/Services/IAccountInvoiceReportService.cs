using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountInvoiceReportService
    {
        Task<IEnumerable<AccountInvoiceReportByTimeItem>> GetSummaryByTime(AccountInvoiceReportByTimeSearch val);
        Task<IEnumerable<AccountInvoiceReportByTimeDetail>> GetDetailByTime(AccountInvoiceReportByTimeItem val);
        Task<AccountInvoiceReportHomeSummaryVM> GetToDaySummary();
        Task<IEnumerable<AccountInvoiceReportAmountResidual>> GetAmountResidualToday();
        Task<IEnumerable<AccountInvoiceReportTopServices>> GetTopServices(int number);
    }
}
