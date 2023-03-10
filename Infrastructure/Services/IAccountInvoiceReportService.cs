using ApplicationCore.Entities;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        IQueryable<AccountInvoiceReport> GetRevenueReportQuery(RevenueReportQueryCommon val);
        Task<IEnumerable<RevenueReportItem>> GetRevenueReport(RevenueReportFilter val);
        Task<RevenueReportExcelVM<RevenueTimeReportExcel>> GetRevenueTimeReportExcel(RevenueTimeReportPar val);
        Task<RevenueReportExcelVM<RevenueServiceReportExcel>> GetRevenueServiceReportExcel(RevenueServiceReportPar val);
        Task<RevenueReportExcelVM<RevenueEmployeeReportExcel>> GetRevenueEmployeeReportExcel(RevenueEmployeeReportPar val);
        Task<RevenueReportExcelVM<RevenuePartnerReportExcel>> GetRevenuePartnerReportExcel(RevenuePartnerReportPar val);

        Task<IEnumerable<RevenueReportItem>> GetRevenueChartReport(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string groupBy, string[] accountCode = null);
        Task<IEnumerable<RevenueTimeReportDisplay>> GetRevenueTimeByMonth(RevenueTimeReportPar val);
        Task<IEnumerable<RevenueDistrictAreaDisplay>> GetRevenueDistrictArea(RevenueDistrictAreaPar val);
    }
}
