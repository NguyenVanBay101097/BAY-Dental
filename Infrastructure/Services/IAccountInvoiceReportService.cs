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
        Task<decimal> SumRevenueReport(SumRevenueReportPar val);
        Task<IEnumerable<RevenueTimeReportDisplay>> GetRevenueTimeReport(RevenueTimeReportPar val);
        Task<IEnumerable<RevenueServiceReportDisplay>> GetRevenueServiceReport(RevenueServiceReportPar val);
        Task<IEnumerable<RevenueEmployeeReportDisplay>> GetRevenueEmployeeReport(RevenueEmployeeReportPar val);
        Task<PagedResult2<RevenueReportDetailDisplay>> GetRevenueReportDetailPaged(RevenueReportDetailPaged val);
        Task<IEnumerable<RevenuePartnerReportDisplay>> GetRevenuePartnerReport(RevenuePartnerReportPar val);
        Task<RevenueReportPrintVM<RevenuePartnerReportPrint>> GetRevenuePartnerReportPrint(RevenuePartnerReportPar val);
        Task<RevenueReportPrintVM<RevenueServiceReportPrint>> GetRevenueServiceReportPrint(RevenueServiceReportPar val);
        Task<RevenueReportPrintVM<RevenueTimeReportPrint>> GetRevenueTimeReportPrint(RevenueTimeReportPar val);
        Task<RevenueReportPrintVM<RevenueEmployeeReportPrint>> GetRevenueEmployeeReportPrint(RevenueEmployeeReportPar val);

    }
}
