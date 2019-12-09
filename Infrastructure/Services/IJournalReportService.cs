using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IJournalReportService
    {
        IEnumerable<JournalReport> GetJournalMoveLineReport(JournalReportPaged paged);
        IEnumerable<AccountMoveLineReport> GetMoveLines(JournalReportDetailPaged val);
    }
}
