using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IReportJournalService
    {
        Task<IEnumerable<ReportJournalItem>> GetCashBankReportValues(ReportCashBankJournalSearch val);
    }
}
