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

        Task<PagedResult2<RevenueTimeReportDisplay>> GetRevenueTimeReportPaged(RevenueTimeReportPaged val);
        Task<PagedResult2<RevenueServiceReportDisplay>> GetRevenueServiceReportPaged(RevenueServiceReportPaged val);
        Task<PagedResult2<RevenueEmployeeReportDisplay>> GetRevenueEmployeeReportPaged(RevenueEmployeeReportPaged val);
        Task<PagedResult2<RevenueReportDetailDisplay>> GetRevenueReportDetailPaged(RevenueReportDetailPaged val);
    }
}
