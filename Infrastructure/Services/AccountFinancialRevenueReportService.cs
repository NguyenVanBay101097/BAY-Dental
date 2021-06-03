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
                .Include(x=> x.FinancialRevenueReportAccountRels)
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
            var accountObj = GetService<IAccountAccountService>();
            var accountTypeObj = GetService<IAccountAccountTypeService>();
            var report = await irModelDataObj.GetRef<AccountFinancialRevenueReport>("report.CreditRevenueReport");
            if (report == null)
            {
                var account_type_thu = await accountTypeObj.GetDefaultAccountTypeThu();

                var acc_revenues = await accountObj.SearchQuery(x => x.Code == "131").ToListAsync();
                var revenuesRel = new List<AccountFinancialRevenueReportAccountAccountRel>();
                foreach (var acc in acc_revenues)
                {
                    revenuesRel.Add(new AccountFinancialRevenueReportAccountAccountRel()
                    {
                        AccountId = acc.Id,
                    });
                }

                var acc_advances = await accountObj.SearchQuery(x => x.Code == "KHTU").ToListAsync();
                var advanceRel = new List<AccountFinancialRevenueReportAccountAccountRel>();
                foreach (var acc in acc_advances)
                {
                    advanceRel.Add(new AccountFinancialRevenueReportAccountAccountRel()
                    {
                        AccountId = acc.Id,
                    });
                }

                var acc_advances2 = await accountObj.SearchQuery(x => x.Code == "KHTU").ToListAsync();
                var advanceRel2 = new List<AccountFinancialRevenueReportAccountAccountRel>();
                foreach (var acc in acc_advances2)
                {
                    advanceRel2.Add(new AccountFinancialRevenueReportAccountAccountRel()
                    {
                        AccountId = acc.Id,
                    });
                }

                var acc_RefSuppliers = await accountObj.SearchQuery(x => x.Code == "331").ToListAsync();
                var refSupplierRel = new List<AccountFinancialRevenueReportAccountAccountRel>();
                foreach (var acc in acc_RefSuppliers)
                {
                    refSupplierRel.Add(new AccountFinancialRevenueReportAccountAccountRel()
                    {
                        AccountId = acc.Id,
                    });
                }

                report = new AccountFinancialRevenueReport()
                {
                    Name = "Báo cáo nguồn thu",
                    Level = 0,
                    Type = "sum",
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
                            FinancialRevenueReportAccountRels = revenuesRel
                        },
                        new AccountFinancialRevenueReport()
                        {
                            Name = "Tạm ứng",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 2,
                            Sign = -1,
                            FinancialRevenueReportAccountRels = advanceRel
                        },
                         new AccountFinancialRevenueReport()
                        {
                            Name = "Thu công nợ",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 3,
                            Sign = -1,
                            //FinancialRevenueReportAccountAccountRels = chờ
                        },
                          new AccountFinancialRevenueReport()
                        {
                            Name = "Nhà cung cấp hoàn tiền",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 4,
                            Sign = -1,
                            FinancialRevenueReportAccountRels = refSupplierRel
                        },
                             new AccountFinancialRevenueReport()
                        {
                            Name = "Nhà cung cấp hoàn tiền",
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
                            FinancialRevenueReportAccountRels = advanceRel2
                        },
                                   new AccountFinancialRevenueReport()
                        {
                            Name = "Thanh toán điều trị bằng ghi công nợ",
                            Level = 2,
                            Type = "account_account",
                            DisplayDetail = "detail_with_hierarchy",
                            Sequence = 2,
                            Sign = -1,
                            //FinancialRevenueReportAccountAccountRels = chờ
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

        public async Task<FinancialRevenueReportItem> getRevenueReport(RevenueReportPar val)
        {
            //lấy ra đối tượng revenue record
            var report = await GetRevenueRecord();
            // biến thành list revenue  include accounttype and acocunt account and childs
            //foreach listRevenue : tính balance
            // response kết quả 
            throw new NotImplementedException();
        }

        public async Task<IDictionary<Guid, ComputeReportBalanceDictValue>> _ComputeReportBalance(IEnumerable<AccountFinancialRevenueReport> reports, RevenueReportPar data)
        {
            var res = new Dictionary<Guid, ComputeReportBalanceDictValue>();
            foreach (var report in reports)
            {
                if (res.ContainsKey(report.Id))
                    continue;
                res.Add(report.Id, new ComputeReportBalanceDictValue());

                if (report.Type == "account_type")
                {
                    var accountObj = GetService<IAccountAccountService>();
                    var accountTypeIds = report.FinancialRevenueReportAccountTypeRels.Where(x => x.FinancialReportId == report.Id).Select(x => x.AccountTypeId).ToList();
                    var spec = new InitialSpecification<AccountAccount>(x => accountTypeIds.Contains(x.UserTypeId) && !x.IsExcludedProfitAndLossReport);
                    var accounts = await accountObj.SearchQuery(spec.AsExpression()).ToListAsync();
                    res[report.Id].Account = await ComputeAccountBalance(accounts, data);

                    foreach (var value in res[report.Id].Account.Values)
                    {
                        res[report.Id].Debit += value.Debit;
                        res[report.Id].Credit += value.Credit;
                        res[report.Id].Balance += value.Balance;
                    }
                }
                else if (report.Type == "sum")
                {
                    //it's the sum of the children of this account.report
                    var res2 = await _ComputeReportBalance(report.Childs, data);
                    foreach (var item in res2.Values)
                    {
                        foreach (var value in item.Account.Values)
                        {
                            res[report.Id].Debit += value.Debit;
                            res[report.Id].Credit += value.Credit;
                            res[report.Id].Balance += value.Balance;
                        }
                    }
                }
            }

            return res;
        }

        public async Task<IDictionary<Guid, ComputeAccountBalanceRes>> ComputeAccountBalance(IEnumerable<AccountAccount> accounts, RevenueReportPar data)
        {
            var res = accounts.ToDictionary(x => x.Id, x => new ComputeAccountBalanceRes
            {
                Debit = 0,
                Credit = 0,
                Balance = 0,
            });

            var accountIds = accounts.Select(x => x.Id).ToList();
            var amlObj = GetService<IAccountMoveLineService>();
            if (accounts.Any())
            {
                ISpecification<AccountMoveLine> filter = amlObj._QueryGetSpec(dateFrom: data.DateFrom, dateTo: data.DateTo, companyId: data.CompanyId);
                filter = filter.And(new InitialSpecification<AccountMoveLine>(x => accountIds.Contains(x.AccountId)));

                var list = await amlObj.SearchQuery(filter.AsExpression()).OrderBy(x => x.Date).GroupBy(x => x.AccountId).Select(x => new ComputeAccountBalanceRes
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
