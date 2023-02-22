using ApplicationCore.Entities;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
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
    public class AccountReportGeneralLedgerService: IAccountReportGeneralLedgerService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountReportGeneralLedgerService(IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        public IEnumerable<ReportGeneralLedgerAccountRes> _GetAccountMoveEntry(IEnumerable<Guid> account_ids, bool init_balance,
            string sortby, string display_account, DateTime? date_from, DateTime? date_to, Guid? company_id)
        {
            var accountObj = GetService<IAccountAccountService>();
            var companyObj = GetService<ICompanyService>();
            var moveLineObj = GetService<IAccountMoveLineService>();

            if (date_to.HasValue)
                date_to = date_to.Value.AbsoluteEndOfDate();

            var move_lines = account_ids.ToDictionary(x => x, x => new List<ReportGeneralLedgerMoveLine>());
            var initial_balance_dict = account_ids.ToDictionary(x => x, x => 0M);

            if (init_balance && date_from.HasValue)
            {
                var domain = moveLineObj._QueryGetSpec(dateFrom: date_from, dateTo: null, initBal: true, state: "posted", companyId: company_id);
                domain = domain.And(new InitialSpecification<AccountMoveLine>(x => account_ids.Contains(x.AccountId)));
                var list = moveLineObj.SearchQuery(domain: domain.AsExpression(), orderBy: x => x.OrderBy(s => s.Date))
                    .GroupBy(x => x.AccountId)
                    .Select(x => new ReportGeneralLedgerMoveLine
                    {
                        AccountId = x.Key,
                        Balance = x.Sum(s => s.Debit) - x.Sum(s => s.Credit),
                    }).ToList();

                foreach (var item in list)
                {
                    initial_balance_dict[item.AccountId] = item.Balance;
                }
            }

            var domain2 = moveLineObj._QueryGetSpec(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: company_id);
            domain2 = domain2.And(new InitialSpecification<AccountMoveLine>(x => account_ids.Contains(x.AccountId)));
            var query2 = moveLineObj.SearchQuery(domain: domain2.AsExpression());
            if (sortby == "sort_journal_partner")
                query2 = query2.OrderBy(x => x.Journal.Code).ThenBy(x => x.Partner.Name).ThenBy(x => x.Move.DateCreated);
            else
                query2 = query2.OrderBy(x => x.Date).ThenBy(x => x.Move.DateCreated);

            var list2 = query2.Select(x => new ReportGeneralLedgerMoveLine
            {
                AccountId = x.AccountId,
                Date = x.Date,
                Name = x.Name,
                Debit = x.Debit,
                Credit = x.Credit,
                Balance = x.Debit - x.Credit,
                MoveName = x.Move.Name,
                PartnerName = x.Partner != null ? x.Partner.Name : string.Empty,
                JournalCode = x.Journal.Code,
                Ref = x.Ref,
            }).ToList();

            foreach (var item in list2)
            {
                if (!move_lines.ContainsKey(item.AccountId))
                    continue;
                decimal balance = initial_balance_dict.ContainsKey(item.AccountId) ? initial_balance_dict[item.AccountId] : 0;
                foreach (var line in move_lines[item.AccountId])
                    balance += (line.Debit - line.Credit);

                item.InitialBalance = balance;
                item.Balance += balance;
                move_lines[item.AccountId].Add(item);
            }

            var accounts = accountObj.SearchQuery(x => account_ids.Contains(x.Id)).Include(x => x.Company).ToList();
            var accountRes = new List<ReportGeneralLedgerAccountRes>();
            foreach (var account in accounts)
            {
                var res = new ReportGeneralLedgerAccountRes
                {
                    InitialBalance = initial_balance_dict[account.Id],
                    Code = account.Code,
                    Name = account.Name,
                    CompanyName = account.Company.Name,
                    MoveLines = move_lines[account.Id],
                };
                foreach (var line in res.MoveLines)
                {
                    res.Debit += line.Debit;
                    res.Credit += line.Credit;
                    res.Balance = line.Balance;
                }

                if (display_account == "all")
                    accountRes.Add(res);

                if (display_account == "movement" && res.MoveLines.Any())
                    accountRes.Add(res);

                if (display_account == "not_zero" && res.Balance != 0)
                    accountRes.Add(res);
            }

            return accountRes;
        }

        public ReportGeneralLedgerValues GetReportValues(AccountReportGeneralLedger data)
        {
            var init_balance = data.InitialBalance ?? true;
            var sortby = data.SortBy ?? "sort_date";
            var display_account = data.DislayAccount;
            var account_ids = data.AccountIds;
            var accounts_res = _GetAccountMoveEntry(account_ids, init_balance, sortby, display_account, data.DateFrom, data.DateTo, data.CompanyId);

            return new ReportGeneralLedgerValues
            {
                Accounts = accounts_res
            };
        }

        public ReportGeneralLedgerValues GetCashBankReportValues(ReportCashBankGeneralLedger val)
        {
            var journalObj = GetService<IAccountJournalService>();
            var types = new string[] { "cash", "bank" };
            if (val.ResultSelection == "cash")
                types = new string[] { "cash" };
            else if (val.ResultSelection == "bank")
                types = new string[] { "bank" };

            var query = journalObj.SearchQuery(x => types.Contains(x.Type));
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId);

            var account_ids = query.Select(x => x.DefaultDebitAccountId.Value).ToList();
            var report = new AccountReportGeneralLedger
            {
                AccountIds = account_ids,
                CompanyId = val.CompanyId,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo,
                InitialBalance = true,
                DislayAccount = "all",
                SortBy = "sort_date",
                TargetMove = "posted"
            };

            return GetReportValues(report);
        }
    }
}
