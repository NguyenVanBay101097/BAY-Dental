using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ICashBookService
    {
        Task<CashBookReport> GetSumary(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string resultSelection);

        Task<PagedResult2<CashBookReportDetail>> GetDetails(DateTime? dateFrom, DateTime? dateTo, int limit, int offset, Guid? companyId, string search, string resultSelection);
        Task<decimal> GetTotal(CashBookSearch val);

        Task<IEnumerable<CashBookReportItem>> GetChartReport(CashBookReportFilter val);

        Task<SumaryCashBook> GetSumaryCashBookReport(SumaryCashBookFilter val);

        Task<IEnumerable<CashBookReportItem>> GetCashBookChartReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string groupBy);
        Task<SumaryCashBook> GetSumaryCashBookReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string partnerType, string accountCode, string resultSelection);

        Task<IEnumerable<DataInvoiceItem>> GetDataInvoices(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string resultSelection);

    }
}
