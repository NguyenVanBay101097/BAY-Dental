using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountFinancialRevenueReportService: IBaseService<AccountFinancialRevenueReport>
    {
        Task<IEnumerable<AccountFinancialRevenueReport>> GetChildren(AccountFinancialRevenueReport report);
        Task<AccountFinancialRevenueReport> GetRevenueRecord();// lấy record báo cáo nguồn thu
        Task<FinancialRevenueReportItem> getRevenueReport(RevenueReportPar val);
    }
}
