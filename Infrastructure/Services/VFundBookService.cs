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

        public IQueryable<AccountMoveLine> _FilterQueryable(VFundBookSearch val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var accMoveLineObj = GetService<IAccountMoveLineService>();
            var query = accMoveLineObj.SearchQuery(x => company_ids.Contains(x.CompanyId.Value));

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

            if (!string.IsNullOrEmpty(val.ResultSelection) && val.ResultSelection != "cash_bank")
                query = query.Where(x => x.Journal.Type.Equals(val.ResultSelection));
            return query;
        }

        public async Task<PagedResult2<VFundBookDisplay>> GetMoney(VFundBookSearch val)
        {
            var query = _FilterQueryable(val);
            var totalItems = await query.CountAsync();
            query = query.OrderByDescending(x => x.Date).Skip(val.Offset).Take(val.Limit);
            var items = await query.Include(x => x.Journal).ToListAsync();

            return new PagedResult2<VFundBookDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<VFundBookDisplay>>(items)
            };
        }

        public async Task<CashBookReport> GetSumary(VFundBookSearch val)
        {
            val.State = "posted";
            var fundBookReport = new CashBookReport();
            //neu co datefrom tinh begin -> select sum trong query1
            if (val.DateFrom.HasValue && val.Begin)
            {
                val.Begin = true;
                var query1 = _FilterQueryable(val);
                if (query1.Any())
                {
                    var thu = query1.Sum(x => x.Debit);
                    var chi = query1.Sum(x => x.Credit);
                    fundBookReport.Begin = thu - chi;
                }
            }

            //loc nhung posted và sum trong query2 tinh dc thu chi
            var query = _FilterQueryable(val);
            if (query.Any())
            {
                fundBookReport.TotalThu = query.Sum(x => x.Debit);
                fundBookReport.TotalChi = query.Sum(x => x.Credit);
            }

            //return total = begin + thu - chi
            fundBookReport.TotalAmount = fundBookReport.Begin + fundBookReport.TotalThu - fundBookReport.TotalChi;

            return fundBookReport;
        }

        public async Task<List<FundBookExportExcel>> GetExportExcel(VFundBookSearch val)
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
