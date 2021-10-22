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
        Task<AccountCommonPartnerReportSearchV2Result> ReportSumaryPartner(AccountCommonPartnerReportSearchV2 data);
        Task<IEnumerable<AccountCommonPartnerReportItem>> ReportSalaryEmployee(AccountCommonPartnerReportSearch val);
        Task<IEnumerable<AccountCommonPartnerReportItemDetail>> ReportSalaryEmployeeDetail(AccountCommonPartnerReportItem val);
        Task<IEnumerable<AccountMoveBasic>> GetListReportPartner(AccountCommonPartnerReportSearch val);
        Task<IEnumerable<ReportPartnerDebitRes>> ReportPartnerDebit(ReportPartnerDebitReq val);
        Task<IEnumerable<ReportPartnerDebitDetailRes>> ReportPartnerDebitDetail(ReportPartnerDebitDetailReq val);

        Task<IEnumerable<ReportPartnerAdvance>> ReportPartnerAdvance(ReportPartnerAdvanceFilter val);
        Task<IEnumerable<ReportPartnerAdvanceDetail>> ReportPartnerAdvanceDetail(ReportPartnerAdvanceDetailFilter val);
        Task<ReportPartnerDebitSummaryRes> ReportPartnerDebitSummary(ReportPartnerDebitReq val);
        Task<ReportPartnerDebitPrintVM> PrintReportPartnerDebit(ReportPartnerDebitReq val);
        Task<AccountCommonPartnerReportPrint> ReportSummaryPrint(AccountCommonPartnerReportSearch val);
        Task<IEnumerable<ReportPartnerDebitExcel>> ExportReportPartnerDebitExcel(ReportPartnerDebitReq val);
    }
}
