using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IReportCustomerDebtService
    {
        Task<PagedResult2<CustomerDebtResult>> GetPagedtCustomerDebtReports(CustomerDebtFilter val);
        Task<CustomerDebtAmountTotal> GetCustomerAmountTotal(AmountCustomerDebtFilter val);
    }
}
