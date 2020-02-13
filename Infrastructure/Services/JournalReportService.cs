using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class JournalReportService : IJournalReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public JournalReportService(CatalogDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        public IEnumerable<JournalReport> GetJournalMoveLineReport(JournalReportPaged paged)
        {
            var journals = _context.AccountJournals;
            var ids = new List<Guid>();

            switch (paged.Filter)
            {
                case "all":
                    ids = journals.Where(x => x.Type == "bank" || x.Type == "cash").Select(x => x.Id).ToList();
                    break;
                case "bank":
                    ids = journals.Where(x => x.Type == "bank").Select(x => x.Id).ToList();
                    break;
                case "cash":
                    ids = journals.Where(x => x.Type == "cash").Select(x => x.Id).ToList();
                    break;
            }

            var moveLines = _context.AccountMoveLines
                .Where(x => ids.Contains(x.JournalId ?? Guid.Empty) && x.Account.InternalType == "liquidity");

            if (paged.DateFrom.HasValue)
                moveLines = moveLines.Where(x => x.Date >= paged.DateFrom.Value.AbsoluteBeginOfDate());
            if (paged.DateTo.HasValue)
                moveLines = moveLines.Where(x => x.Date <= paged.DateTo.Value.AbsoluteEndOfDate());

            var group = moveLines.GroupBy(x => new
            {
                x.JournalId,
                JournalName = x.Journal.Name

            }).Select(x => new JournalReport
            {
                Name = x.Key.JournalName,
                DebitSum = x.Sum(y => y.Debit),
                CreditSum = x.Sum(y => y.Credit),
                BalanceSum = x.Sum(y => y.Debit - y.Credit),
                GroupBy = paged.GroupBy,

                JournalId = x.Key.JournalId
            });

            if (!string.IsNullOrEmpty(paged.Search))
                group = group.Where(x => x.Name.Contains(paged.Search));

            var total = group.ToList().Count();
            return group.ToList();

        }

        public IEnumerable<AccountMoveLineReport> GetMoveLines(JournalReportDetailPaged val)
        {
            var companyId = CompanyId;
            var query = _context.AccountMoveLines.Where(x => x.Account.InternalType == "liquidity" 
            && x.CompanyId == companyId && x.JournalId.Equals(val.JournalId));

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value.AbsoluteBeginOfDate());
            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value.AbsoluteEndOfDate());

            if (val.GroupBy == "date:month")
            {
                var group = query.GroupBy(x => new
                {
                    x.Date.Value.Year,
                    x.Date.Value.Month

                }).Select(x => new AccountMoveLineReport
                {
                    Name = $"{x.Key.Month}/{x.Key.Year}",
                    Debit = x.Sum(y => y.Debit),
                    Credit = x.Sum(y => y.Credit),
                    Balance = x.Sum(y => y.Debit - y.Credit)
                });
                return group.ToList();
            }
            else if (val.GroupBy == "date:quarter")
            {
                var group = query.GroupBy(x => new
                {
                    x.Date.Value.Year,
                    Quarter = (x.Date.Value.Month - 1) / 3 + 1,

                }).Select(x => new AccountMoveLineReport
                {
                    Name = $"Quý {x.Key.Quarter}, {x.Key.Year}",
                    Debit = x.Sum(y => y.Debit),
                    Credit = x.Sum(y => y.Credit),
                    Balance = x.Sum(y => y.Debit - y.Credit)
                });
                return group.ToList();
            }
            else if (val.GroupBy == "date:week")
            {
                var group = query.GroupBy(x => _context.DatePart("week", x.Date)).Select(x => new AccountMoveLineReport
                {
                    Name = $"Tuần {x.Key}",
                    Debit = x.Sum(y => y.Debit),
                    Credit = x.Sum(y => y.Credit),
                    Balance = x.Sum(y => y.Debit - y.Credit)
                });
                return group.ToList();
            }
            else
            {
                var group = query.GroupBy(x => new
                {
                    x.Date.Value.Year,
                    x.Date.Value.Month,
                    x.Date.Value.Day,
                }).Select(x => new AccountMoveLineReport
                {
                    Name = $"{x.Key.Day}/{x.Key.Month}/{x.Key.Year}",
                    Debit = x.Sum(y => y.Debit),
                    Credit = x.Sum(y => y.Credit),
                    Balance = x.Sum(y => y.Debit - y.Credit)
                });
                return group.ToList();
            }
        }
    }
}
