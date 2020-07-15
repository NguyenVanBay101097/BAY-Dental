
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
    public class ReportFinancialService : BaseService<AccountFinancialReport>, IReportFinancialService
    {
        public ReportFinancialService(
            IAsyncRepository<AccountFinancialReport> repository,
            IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
        }

        public async Task<IEnumerable<GetAccountLinesItem>> GetAccountLines(AccountingReport data)
        {
            var accountFinancialReport = await GetByDisplayDetail(data.KeyDisplayDetail);
            var lines = new List<GetAccountLinesItem>();
            if (accountFinancialReport != null)
            {
                var accountReport = await SearchQuery(x => x.Id == accountFinancialReport.Id).Include(x => x.FinancialReportAccountTypeRels).FirstOrDefaultAsync();
                var childReports = await _GetChildrenByOrder(accountReport);
                var res = await _ComputeReportBalance(childReports, data);
                foreach (var report in childReports)
                {
                    var vals = new GetAccountLinesItem
                    {
                        Name = report.Name,
                        Balance = res[report.Id].Balance * report.Sign,
                        Type = "report",
                        Level = report.Level,
                        AccountType = report.Type
                    };
                    if (data.DebitCredit == true)
                    {
                        vals.Debit = res[report.Id].Debit;
                        vals.Credit = res[report.Id].Credit;
                    }

                    lines.Add(vals);
                    if (report.DisplayDetail == "no_detail")
                        continue;

                    if (res[report.Id].Account.Count() > 0)
                    {
                        var subLines = new List<GetAccountLinesItem>();
                        var i = 0;
                        foreach (var item in res[report.Id].Account)
                        {

                            var accountId = item.Key;
                            var value = item.Value;
                            var flag = false;
                            var accountObj = GetService<IAccountAccountService>();
                            var account = await accountObj.GetByIdAsync(accountId);

                            var subVals = new GetAccountLinesItem()
                            {
                                Name = account.Code + " " + account.Name,
                                Balance = value.Balance * report.Sign,
                                Type = "account",
                                Level = report.DisplayDetail == "detail_with_hierarchy" ? 4 : 0,
                                AccountType = account.InternalType,
                            };
                            if (data.DebitCredit == true)
                            {
                                subVals.Debit = value.Debit;
                                subVals.Credit = value.Credit;
                                if (subVals.Debit != 0 || subVals.Credit != 0)
                                    flag = true;
                            }

                            if (subVals.Balance != 0)
                                flag = true;
                            if (flag)
                                subLines.Add(subVals);
                            i++;
                        }

                        lines.AddRange(subLines.OrderBy(x => x.Name));
                    }
                }
            }
            return lines;
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

        public async Task<IDictionary<Guid, ComputeReportBalanceDictValue>> _ComputeReportBalance(IEnumerable<AccountFinancialReport> reports, AccountingReport data)
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
                    var accountTypeIds = report.FinancialReportAccountTypeRels.Where(x => x.FinancialReportId == report.Id).Select(x => x.AccountTypeId).ToList();
                    var spec = new InitialSpecification<AccountAccount>(x => accountTypeIds.Any(s => s == x.UserTypeId));
                    var accounts = await accountObj.SearchQuery(spec.AsExpression()).ToListAsync();
                    res[report.Id].Account = await _ComputeAccountBalance(accounts, data);

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

        public async Task<IDictionary<Guid, ComputeAccountBalanceRes>> _ComputeAccountBalance(IEnumerable<AccountAccount> accounts, AccountingReport data)
        {
            var res = accounts.ToDictionary(x => x.Id, x => new ComputeAccountBalanceRes
            {
                Debit = 0,
                Credit = 0,
                Balance = 0,
            });

            var accountIds = accounts.Select(x => x.Id).ToList();
            var amlObj = GetService<IAccountMoveLineService>();
            if (accounts.Count() > 0)
            {
                ISpecification<AccountMoveLine> spec = new InitialSpecification<AccountMoveLine>(x => accountIds.Any(s => s == x.AccountId));

                if (data.DateFrom.HasValue)
                    spec = spec.And(new InitialSpecification<AccountMoveLine>(x => x.Date >= data.DateFrom));

                if (data.DateTo.HasValue)
                    spec = spec.And(new InitialSpecification<AccountMoveLine>(x => x.Date <= data.DateTo));

                if (data.CompanyId.HasValue)
                    spec = spec.And(new InitialSpecification<AccountMoveLine>(x => x.CompanyId == data.CompanyId));

                var list = await amlObj.SearchQuery(spec.AsExpression()).OrderBy(x => x.Date).GroupBy(x => x.AccountId).Select(x => new ComputeAccountBalanceRes
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

        public async Task<AccountFinancialReport> GetByDisplayDetail(string key)
        {
            var model = await SearchQuery(x => x.DisplayDetail.Equals(key)).FirstOrDefaultAsync();
            return model;
        }

        public class ComputeAccountBalanceRes
        {
            public Guid AccountId { get; set; }

            public decimal? Balance { get; set; }

            public decimal? Debit { get; set; }

            public decimal? Credit { get; set; }
        }

        public class ComputeReportBalanceDictValue
        {
            public ComputeReportBalanceDictValue()
            {
                Account = new Dictionary<Guid, ComputeAccountBalanceRes>();
                Debit = 0;
                Credit = 0;
                Balance = 0;
            }
            public IDictionary<Guid, ComputeAccountBalanceRes> Account { get; set; }

            public decimal? Debit { get; set; }

            public decimal? Credit { get; set; }

            public decimal? Balance { get; set; }
        }

        public class GetAccountLinesItem
        {
            public GetAccountLinesItem()
            {
                Balance = 0;
                Debit = 0;
                Credit = 0;
            }
            public string Name { get; set; }

            public decimal? Balance { get; set; }

            public string Type { get; set; }

            public int? Level { get; set; }

            public string AccountType { get; set; }

            public decimal? Debit { get; set; }

            public decimal? Credit { get; set; }
        }
    }
}


