using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITCareReportService
    {
        Task<List<TCareReports>> GetReportTCare(TCareScenarioFilterReport val);
        Task<List<TCareReportsItem>> GetReportTCareDetail(TCareReports val);
    }
}
