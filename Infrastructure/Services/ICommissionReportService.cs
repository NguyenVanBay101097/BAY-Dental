using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using static Infrastructure.Services.CommissionReportService;

namespace Infrastructure.Services
{
    public interface ICommissionReportService
    {
        Task<IEnumerable<CommissionReport>> GetReport(ReportFilterCommission val);

        Task<IEnumerable<CommissionReportItem>> GetReportDetail(ReportFilterCommissionDetail val);
    }
}
