using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IPartnerOldNewReportService
    {
        Task<IEnumerable<PartnerOldNewReportVM>> GetPartnerOldNewReport(PartnerOldNewReportSearch val);

        Task<PartnerOldNewReportVM> GetSumaryPartnerOldNewReport(PartnerOldNewReportSearch val);
        Task<PagedResult2<PartnerOldNewReportRes>> GetReport(PartnerOldNewReportReq val);
        Task<int> SumReport(PartnerOldNewReportReq val);
        Task<PartnerOldNewReportPrint> GetReportPrint(PartnerOldNewReportReq val);
        Task<decimal> SumReVenue(PartnerOldNewReportReq val);
        Task<IEnumerable<PartnerOldNewReportByWard>> ReportByWard(PartnerOldNewReportByWardReq val);
        Task<PagedResult2<SaleOrderBasic>> GetSaleOrderPaged(GetSaleOrderPagedReq val);
        Task<PartnerOldNewReportExcel> GetReportExcel(PartnerOldNewReportReq val);
        Task<IEnumerable<PartnerOldNewReportByIsNewItem>> ReportByIsNew(PartnerOldNewReportByIsNewReq val);
    }


}
