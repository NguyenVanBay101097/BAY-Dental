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
        Task<IEnumerable<PartnerOldNewReportRes>> GetReport(PartnerOldNewReportReq val);
        Task<int> SumReport(PartnerOldNewReportSumReq val);
    }


}
