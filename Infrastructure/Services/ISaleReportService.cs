using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISaleReportService
    {
        Task<IEnumerable<SaleReportTopServicesCs>> GetTopServices(SaleReportTopServicesFilter val);
        Task<IEnumerable<SaleReportItem>> GetReport(SaleReportSearch val);
        Task<IEnumerable<SaleReportItem>> GetReportForSmsMessage(IEnumerable<Guid> partnerIds);
        Task<IEnumerable<SaleReportItemDetail>> GetReportDetail(SaleReportItem val);
        //Task<IEnumerable<SaleReportPartnerItem>> GetReportPartner(SaleReportPartnerSearch val);
        Task<IEnumerable<SaleReportPartnerItem>> GetReportPartnerV2(SaleReportPartnerSearch val);
        Task<IEnumerable<SaleReportPartnerItemV3>> GetReportPartnerV3(SaleReportPartnerSearch val);
        Task<IEnumerable<SaleReportPartnerItemV3>> GetReportPartnerV4(SaleReportPartnerSearch val);
        Task<IEnumerable<SaleReportItem>> GetTopSaleProduct(SaleReportTopSaleProductSearch val);
        Task<PagedResult2<SaleOrderLineDisplay>> GetReportService(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string search, string state);
        Task<SaleReportOldNewPartnerOutput> GetReportOldNewPartner(SaleReportOldNewPartnerInput val);
        Task<IEnumerable<ServiceReportRes>> GetServiceReportByTime(ServiceReportReq val);
        Task<IEnumerable<ServiceReportRes>> GetServiceReportByService(ServiceReportReq val);
        Task<PagedResult2<ServiceReportDetailRes>> GetServiceReportDetailPaged(ServiceReportDetailReq val);

        Task<IEnumerable<ServiceOverviewResponse>> ServiceOverviewReport(ServiceReportReq val);
        Task<PrintServiceOverviewResponse> PrintServiceOverviewReport(SaleOrderLinesPaged val);
        Task<ServiceReportPrint> ServiceReportByServicePrint(ServiceReportReq val);
        Task<ServiceReportPrint> ServiceReportByTimePrint(ServiceReportReq val);
        Task<IEnumerable<ServiceReportResExcel>> ServiceReportByTimeExcel(ServiceReportReq val);
        Task<IEnumerable<ServiceReportResExcel>> ServiceReportByServiceExcel(ServiceReportReq val);
        FileContentResult ExportServiceReportExcel(IEnumerable<ServiceReportResExcel> data, DateTime? dateFrom, DateTime? dateTo, string type);
    }
}
