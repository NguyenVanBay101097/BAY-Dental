using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountCommonPartnerReportService
    {
        Task<IEnumerable<AccountCommonPartnerReportItem>> ReportSummary(AccountCommonPartnerReportSearch val);
        Task<IEnumerable<AccountCommonPartnerReportItemDetail>> ReportDetail(AccountCommonPartnerReportItem val);
    }
}
