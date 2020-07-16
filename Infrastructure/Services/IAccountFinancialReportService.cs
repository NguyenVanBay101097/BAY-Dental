using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountFinancialReportService : IBaseService<AccountFinancialReport>
    {
        Task<IEnumerable<AccountFinancialReport>> _GetChildrenByOrder(AccountFinancialReport report);

        Task<AccountFinancialReport> GetProfitAndLossReport();
    }
}
