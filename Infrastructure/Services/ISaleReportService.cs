using ApplicationCore.Models;
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
        Task<PagedResult2<SaleOrderLineDisplay>> GetReportService(SaleReportSearch val);
        Task<SaleReportOldNewPartnerOutput> GetReportOldNewPartner(SaleReportOldNewPartnerInput val);
    }
}
