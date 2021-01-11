using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Data;
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
    public class VFundBookService : IVFundBookService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VFundBookService(CatalogDbContext context, IMapper mapper,
           IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
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

        public IQueryable<AccountMoveLine> _FilterQueryable(CashBookSearch val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var accMoveLineObj = GetService<IAccountMoveLineService>();
            var query = accMoveLineObj.SearchQuery(x => company_ids.Contains(x.CompanyId.Value) && x.AccountInternalType == "liquidity");

            var types = new string[] { "cash", "bank" };
            if (val.ResultSelection == "cash")
                types = new string[] { "cash" };
            else if (val.ResultSelection == "bank")
                types = new string[] { "bank" };

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (val.DateFrom.HasValue && !val.Begin)
                query = query.Where(x => x.Date >= val.DateFrom.Value);

            if (val.DateTo.HasValue && !val.Begin)
                query = query.Where(x => x.Date <= val.DateTo.Value);

            if (val.DateFrom.HasValue && val.Begin)
                query = query.Where(x => x.Date < val.DateFrom.Value);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (!string.IsNullOrEmpty(val.ResultSelection))
                query = query.Where(x => types.Contains(x.Journal.Type));

            return query;
        }

        public async Task<PagedResult2<AccountMoveLineCashBookVM>> GetMoney(CashBookSearch val)
        {
            var query = _FilterQueryable(val);
            var totalItems = await query.CountAsync();
            query = query.OrderByDescending(x => x.Date).Skip(val.Offset).Take(val.Limit);
            var items = await query.Select(x => new AccountMoveLineCashBookVM()
            {
                PartnerName = x.Partner.Name,
                Name = x.Name,
                CompanyId = x.CompanyId.HasValue ? x.CompanyId.Value : Guid.Empty,
                Credit = x.Credit,
                Date = x.Date.HasValue ? x.Date.Value : DateTime.Now,
                Debit = x.Debit,
                Ref = x.Ref,
                JournalType = x.Journal.Type
            }).ToListAsync();

            return new PagedResult2<AccountMoveLineCashBookVM>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<CashBookReport> GetSumary(CashBookSearch val)
        {
            var cashBookReport = new CashBookReport();
            //neu co datefrom tinh begin -> select sum trong query1
            if (val.DateFrom.HasValue)
            {
                val.Begin = true;
                var query1 = _FilterQueryable(val);
                if (query1.Any())
                {
                    var thu = query1.Sum(x => x.Debit);
                    var chi = query1.Sum(x => x.Credit);
                    cashBookReport.Begin = thu - chi;
                }
                val.Begin = false;
            }

            //loc nhung posted và sum trong query2 tinh dc thu chi
            var query = _FilterQueryable(val);
            if (query.Any())
            {
                cashBookReport.TotalThu = query.Sum(x => x.Debit);
                cashBookReport.TotalChi = query.Sum(x => x.Credit);
            }

            //return total = begin + thu - chi
            cashBookReport.TotalAmount = cashBookReport.Begin + cashBookReport.TotalThu - cashBookReport.TotalChi;

            return cashBookReport;
        }

        public async Task<CashBookReport> GetTotalReport(CashBookSearch val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var accMoveLineObj = GetService<IAccountMoveLineService>();
            var cashBookReport = new CashBookReport();
            var query = accMoveLineObj.SearchQuery(x => company_ids.Contains(x.CompanyId.Value) && x.AccountInternalType == "liquidity");
            var types = new string[] { "cash", "bank" };
            if (val.ResultSelection == "cash")
                types = new string[] { "cash" };
            else if (val.ResultSelection == "bank")
                types = new string[] { "bank" };
            if (!string.IsNullOrEmpty(val.ResultSelection))
                query = query.Where(x => types.Contains(x.Journal.Type));
            if (query.Any())
            {
                cashBookReport.TotalAmount = query.Sum(x => x.Debit) - query.Sum(x => x.Credit);
            }
            return cashBookReport;
        }

        public async Task<List<FundBookExportExcel>> GetExportExcel(CashBookSearch val)
        {
            //var query = _FilterQueryable(val);
            //var totalItems = await query.CountAsync();
            //query = query.Where(x => x.State == "posted").OrderByDescending(x => x.Date).Skip(val.Offset).Take(val.Limit);
            //var items = await query.Include(x => x.Journal).ToListAsync();

            //var res = items.Select(x => new FundBookExportExcel
            //{
            //    Date = x.Date,
            //    Name = x.Name,
            //    Type = x.Type,
            //    Type2 = x.Type2,
            //    Amount = x.Amount,
            //    RecipientPayer = x.RecipientPayer,
            //    State = x.State
            //}).ToList();

            //return res;
            return null;
        }
    }
}
