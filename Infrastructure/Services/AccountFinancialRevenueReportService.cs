using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class AccountFinancialRevenueReportService : BaseService<AccountFinancialRevenueReport>, IAccountFinancialRevenueReportService
    {
        public AccountFinancialRevenueReportService(IAsyncRepository<AccountFinancialRevenueReport> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {

        }

        public async Task<IEnumerable<AccountFinancialRevenueReport>> GetChildren(AccountFinancialRevenueReport report)
        {
            var res = new List<AccountFinancialRevenueReport>() { report };
            var children = await SearchQuery(x => x.ParentId == report.Id, orderBy: x => x.OrderBy(s => s.Sequence))
                .Include(x => x.FinancialRevenueReportAccountRels)
                .Include(x => x.FinancialRevenueReportAccountTypeRels).ToListAsync();
            if (children.Count() > 0)
            {
                foreach (var child in children)
                {
                    res.AddRange(await GetChildren(child));
                }
            }
            return res;
        }

        public async Task<AccountFinancialRevenueReport> GetRevenueRecord()
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var accountTypeObj = GetService<IAccountAccountTypeService>();
            var report = await irModelDataObj.GetRef<AccountFinancialRevenueReport>("report.CreditRevenueReport");
            if (report == null)
            {
                var account_type_thu = await accountTypeObj.GetDefaultAccountTypeThu();

                report = new AccountFinancialRevenueReport()
                {
                    Name = "Báo cáo nguồn thu",
                    Level = 0,
                    Type = "difference",
                    DisplayDetail = "detail_flat",
                    Sequence = 1,
                    Sign = -1,
                    Childs = new List<AccountFinancialRevenueReport>()
                    {
                        new AccountFinancialRevenueReport()
                        {
                             Name = "Khoản cộng thực thu",
                    Level = 1,
                    Type = "sum",
                    DisplayDetail = "detail_flat",
                    Sequence = 1,
                    Sign = -1,
                    Childs = new List<AccountFinancialRevenueReport>()
                    {
                        new AccountFinancialRevenueReport()
                        {
                            Name = "Doanh thu",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 1,
                            Sign = -1,
                            FinancialRevenueReportAccountRels = new List<AccountFinancialRevenueReportAccountAccountRel>()
                                {
                                     new AccountFinancialRevenueReportAccountAccountRel()
                                     {
                                        AccountCode = "131",
                                         Column = 1,
                                         JournalTypes = "cash,bank"
                                      }
                                 }
                        },
                        new AccountFinancialRevenueReport()
                        {
                            Name = "Tạm ứng",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 2,
                            Sign = -1,
                            FinancialRevenueReportAccountRels = new List<AccountFinancialRevenueReportAccountAccountRel>() {
                                           new AccountFinancialRevenueReportAccountAccountRel()
                                           {
                                               AccountCode = "KHTU",
                                               Column = 1,
                                               JournalTypes = "cash,bank"
                                           }
                                           }
                        },
                         new AccountFinancialRevenueReport()
                        {
                            Name = "Thu công nợ",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 3,
                            Sign = -1,
                            //FinancialRevenueReportAccountAccountRels = chờ column cộng
                        },
                          new AccountFinancialRevenueReport()
                        {
                            Name = "Nhà cung cấp hoàn tiền",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 4,
                            Sign = -1,
                            FinancialRevenueReportAccountRels = new List<AccountFinancialRevenueReportAccountAccountRel>(){
                                                    new AccountFinancialRevenueReportAccountAccountRel()
                                                {
                                                    AccountCode = "331",
                                                    Column = 2,
                                                     JournalTypes = "cash,bank"
                                                } }
                         },
                             new AccountFinancialRevenueReport()
                        {
                            Name = "Thu ngoài",
                            Level = 2,
                            Type = "account_type",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 5,
                            Sign = -1,
                            FinancialRevenueReportAccountTypeRels = new List<AccountFinancialRevenueReportAccountAccountTypeRel>()
                            {
                                new AccountFinancialRevenueReportAccountAccountTypeRel()
                                {
                                    AccountTypeId = account_type_thu.Id,
                                    Column = 1,
                                    JournalTypes = "cash,bank"
                                },
                            }
                        },

                    }
                        },
                        new AccountFinancialRevenueReport()
                        {
                            Name = "Khoản trừ thực thu",
                            Level = 1,
                            Type = "sum",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 2,
                            Sign = -1,
                            Childs=  new List<AccountFinancialRevenueReport>() {
                                 new AccountFinancialRevenueReport()
                        {
                            Name = "Thanh toán điều trị bằng tạm ứng",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 1,
                            Sign = -1,
                            FinancialRevenueReportAccountRels =  new List<AccountFinancialRevenueReportAccountAccountRel>() {
                                           new AccountFinancialRevenueReportAccountAccountRel()
                                           {
                                               AccountCode = "KHTU",
                                               Column = 2,
                                               JournalTypes = "advance"
                                           }
                                           }
                        },
                                   new AccountFinancialRevenueReport()
                        {
                            Name = "Thanh toán điều trị bằng ghi công nợ",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 2,
                            Sign = -1,
                            //FinancialRevenueReportAccountAccountRels = chờ column trừ
                        }
                            }
                        }
                    }

                };

                await CreateAsync(report);

                await irModelDataObj.CreateAsync(new IRModelData()
                {
                    ResId = report.Id.ToString(),
                    Model = "account.financialRevenue.report",
                    Module = "report",
                    Name = "CreditRevenueReport"
                });
            }
            return report;
        }

        private FinancialRevenueReportItem FinancialRevenueReportRes(AccountFinancialRevenueReport report, IDictionary<Guid, decimal> dict)
        {
            var res = new FinancialRevenueReportItem()
            {
                Balance = dict[report.Id],
                Level = report.Level,
                Name = report.Name,
                Sequence = report.Sequence,
                Type = report.Type,
            };
            foreach (var child in report.Childs)
            {
                var childRes = FinancialRevenueReportRes(child, dict);
                childRes.ParentId = report.Id;
                res.Childs.Add(childRes);
            }
            return res;
        }

        public async Task<FinancialRevenueReportItem> getRevenueReport(RevenueReportPar val)
        {
            //lấy ra đối tượng revenue record
            var report = await GetRevenueRecord();
            // biến thành list revenue  include accounttype and acocunt account and childs
            var childReports = await GetChildren(report);
            //foreach listRevenue : tính balance và response kết quả
            var chilReportBalanceDict = await this._ComputeReportBalance(childReports, val);
            var res = FinancialRevenueReportRes(report, chilReportBalanceDict);
            return res;
        }

        private decimal SumComputeAccountBalanceResColumn(IEnumerable<ComputeAccountBalanceRes> datas, int column)
        {
            var res = 0.0M;
            foreach (var data in datas)
            {
                switch (column)
                {
                    case 1:
                        res += data.Credit.Value;
                        break;
                    case 2:
                        res += data.Debit.Value;
                        break;
                    case 3:
                        res += data.Balance.Value;
                        break;
                    default:
                        res += 0;
                        break;
                }
            }
            return res;
        }

        public async Task<IDictionary<Guid, decimal>> _ComputeReportBalance(IEnumerable<AccountFinancialRevenueReport> reports, RevenueReportPar data)
        {
            var res = new Dictionary<Guid, decimal>();
            foreach (var report in reports)
            {
                if (res.ContainsKey(report.Id))
                    continue;
                res.Add(report.Id, 0);

                if (report.Type == "account_type")
                {
                    var accountObj = GetService<IAccountAccountService>();
                    foreach (var accTypeRel in report.FinancialRevenueReportAccountTypeRels)
                    {
                        var accounts = await accountObj.SearchQuery(x => accTypeRel.AccountTypeId == x.UserTypeId && !x.IsExcludedProfitAndLossReport).ToListAsync();
                        var valueDict = await ComputeAccountBalance(accounts, data, accTypeRel.JournalTypes);
                        res[report.Id] += SumComputeAccountBalanceResColumn(valueDict.Values, accTypeRel.Column);
                    }
                }
                else if (report.Type == "account_account")
                {
                    var accountObj = GetService<IAccountAccountService>();
                    foreach (var accAccRel in report.FinancialRevenueReportAccountRels)
                    {
                        var accounts = await accountObj.SearchQuery(x => x.Code == accAccRel.AccountCode && !x.IsExcludedProfitAndLossReport).ToListAsync();
                        var valueDict = await ComputeAccountBalance(accounts, data, accAccRel.JournalTypes);
                        res[report.Id] += SumComputeAccountBalanceResColumn(valueDict.Values, accAccRel.Column);
                    }
                }
                else if (report.Type == "sum")
                {
                    //it's the sum of the children of this account.report
                    var res2 = await _ComputeReportBalance(report.Childs, data);
                    foreach (var item in res2.Values)
                    {
                        res[report.Id] += item;
                    }
                }
                else if (report.Type == "difference")
                {
                    //it's the difference of the children of this account.report
                    var res2 = await _ComputeReportBalance(report.Childs, data);
                    var valueArr = res2.Values.ToList();
                    for (int i = 0; i < valueArr.Count; i++)
                    {
                        if (i == 0)
                            res[report.Id] += valueArr[i];
                        else
                            res[report.Id] -= valueArr[i];
                    }
                }
            }

            return res;
        }

        public async Task<IDictionary<Guid, ComputeAccountBalanceRes>> ComputeAccountBalance(IEnumerable<AccountAccount> accounts, RevenueReportPar data, string journalTypes = null)
        {
            var res = accounts.ToDictionary(x => x.Id, x => new ComputeAccountBalanceRes
            {
                Debit = 0,
                Credit = 0,
                Balance = 0,
            });

            var accountIds = accounts.Select(x => x.Id).ToList();
            var amlObj = GetService<IAccountMoveLineService>();
            var journalObj = GetService<IAccountJournalService>();

            var journalTypeList = new List<string>();
            if (!string.IsNullOrEmpty(journalTypes))
                journalTypeList = journalTypes.Split(",").ToList();

            if (accounts.Any())
            {
                var jounals = await journalObj.SearchQuery(x => journalTypeList.Contains(x.Type)).ToListAsync();
                var journalIds = jounals.Select(x => x.Id);

                ISpecification<AccountMoveLine> filter = amlObj._QueryGetSpec(dateFrom: data.DateFrom, dateTo: data.DateTo, companyId: data.CompanyId);
                filter = filter.And(new InitialSpecification<AccountMoveLine>(x => accountIds.Contains(x.AccountId)));

                var list = await amlObj.SearchQuery(filter.AsExpression()).Where(x => journalIds.Contains(x.JournalId.Value) || journalTypeList.Count == 0).OrderBy(x => x.Date).GroupBy(x => x.AccountId).Select(x => new ComputeAccountBalanceRes
                {
                    AccountId = x.Key,
                    Debit = x.Sum(s => s.Debit),
                    Credit = x.Sum(s => s.Credit),
                    Balance = x.Sum(s => s.Debit) - x.Sum(s => s.Credit)
                }).ToListAsync();

                foreach (var item in list)
                    res[item.AccountId] = item;
            }

            return res;
        }

    }
}
