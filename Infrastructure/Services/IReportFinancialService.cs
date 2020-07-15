using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using static Infrastructure.Services.ReportFinancialService;

namespace Infrastructure.Services
{
    public interface IReportFinancialService : IBaseService<AccountFinancialReport>
    {
        Task<IEnumerable<GetAccountLinesItem>> GetAccountLines(AccountingReport data);
        Task<AccountFinancialReport> GetByDisplayDetail(string key);
    }
}
