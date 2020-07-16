using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AccountFinancialReportService : BaseService<AccountFinancialReport>, IAccountFinancialReportService
    {
        public AccountFinancialReportService(IAsyncRepository<AccountFinancialReport> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
        }

        public async Task<AccountFinancialReport> GetProfitAndLossReport()
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var report = await irModelDataObj.GetRef<AccountFinancialReport>("report.profit_and_loss_report");
            if (report == null)
            {
                var account_type_revenue = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_revenue");
                var account_type_expenses = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_expenses");

                report = new AccountFinancialReport()
                {
                    Name = "Báo cáo lợi nhuận",
                    Level = 0,
                    Type = "sum",
                    DisplayDetail = "detail_flat",
                    Sequence = 1,
                    Sign = -1,
                    Childs = new List<AccountFinancialReport>()
                    {
                        new AccountFinancialReport()
                        {
                            Name = "Doanh thu",
                            Level = 1,
                            Type = "account_type",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 1,
                            Sign = -1,
                            FinancialReportAccountTypeRels = new List<AccountFinancialReportAccountAccountTypeRel>()
                            {
                                new AccountFinancialReportAccountAccountTypeRel()
                                {
                                    AccountTypeId = account_type_revenue.Id,
                                }
                            }
                        },
                        new AccountFinancialReport()
                        {
                            Name = "Chi phí",
                            Level = 1,
                            Type = "account_type",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 2,
                            Sign = -1,
                             FinancialReportAccountTypeRels = new List<AccountFinancialReportAccountAccountTypeRel>()
                            {
                                new AccountFinancialReportAccountAccountTypeRel()
                                {
                                    AccountTypeId = account_type_expenses.Id,
                                }
                            }
                        }
                    }
                };

                await CreateAsync(report);

                await irModelDataObj.CreateAsync(new IRModelData()
                {
                    ResId = report.Id.ToString(),
                    Model = "account.financial.report",
                    Module = "report",
                    Name = "profit_and_loss_report"
                });
            }
            return report;
        }

        public async Task<IEnumerable<AccountFinancialReport>> _GetChildrenByOrder(AccountFinancialReport report)
        {
            var res = new List<AccountFinancialReport>() { report };
            var children = await SearchQuery(x => x.ParentId == report.Id, orderBy: x => x.OrderBy(s => s.Sequence)).Include(x => x.FinancialReportAccountTypeRels).ToListAsync();
            if (children.Count() > 0)
            {
                foreach (var child in children)
                {
                    res.AddRange(await _GetChildrenByOrder(child));
                }
            }
            return res;
        }
    }
}
