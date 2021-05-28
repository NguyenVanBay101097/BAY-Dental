using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountInvoiceReportService
    {

        Task<PagedResult2<AccountInvoiceReportDisplay>> GetRevenueReportPaged(AccountInvoiceReportPaged val);
        Task<PagedResult2<AccountInvoiceReportDetailDisplay>> GetRevenueReportDetailPaged(AccountInvoiceReportDetailPaged val);
    }
}
