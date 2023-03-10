using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IResInsuranceReportService
    {
        Task<IEnumerable<InsuranceDebtReport>> GetInsuranceDebtReport(InsuranceDebtFilter val);

        Task<IEnumerable<InsuranceReportItem>> ReportSummary(InsuranceReportFilter val);

        Task<IEnumerable<InsuranceReportDetailItem>> ReportDetail(InsuranceReportDetailFilter val);

        Task<InsuranceReportPrint> ReportSummaryPrint(InsuranceReportFilter val);

        Task<IEnumerable<ReportInsuranceDebitExcel>> ExportReportInsuranceDebtExcel(InsuranceReportFilter val);

        Task<PagedResult2<InsuranceHistoryInCome>> GetHistoryInComePaged(InsuranceHistoryInComeFilter val);

        Task<InsuranceHistoryInComeDetailItem> GetHistoryInComeDetail(InsuranceHistoryInComeDetailFilter val);
        Task<IEnumerable<InsuranceDebtDetailItem>> GetInsuranceDebtDetailReport(InsuranceDebtDetailFilter val);
    }
}
