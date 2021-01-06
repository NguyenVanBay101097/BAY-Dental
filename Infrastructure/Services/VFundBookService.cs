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

        public IQueryable<VFundBook> _FilterQueryable(VFundBookSearch val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var query = _context.VFundBooks.Where(x => company_ids.Contains(x.CompanyId));

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (!string.IsNullOrEmpty(val.State) && val.State != "all")
                query = query.Where(x => x.State == val.State);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);

            if (!string.IsNullOrEmpty(val.Type))
                query = query.Where(x => x.Type.Equals(val.Type));

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

        public async Task<FundBookReport> GetSumary(VFundBookSearch val)
        {
            val.State = "posted";
            var fundBookReport = new FundBookReport();
            //neu co datefrom tinh begin -> select sum trong query1
            if (val.DateFrom.HasValue && val.Begin)
            {
                IQueryable<VFundBook> query1 = _context.VFundBooks.Where(x => x.Date < val.DateFrom && val.State == "posted");
                if (!string.IsNullOrEmpty(val.ResultSelection) && val.ResultSelection != "cash_bank")
                    query1 = query1.Where(x => x.Journal.Type == val.ResultSelection);
                if (query1.Any())
                    fundBookReport.Begin = query1.Sum(x => x.AmountSigned);
                else
                    fundBookReport.Begin = 0;
            }

            //loc nhung posted và sum trong query2 tinh dc thu chi
            var query = _FilterQueryable(val);
            if (query.Any())
            {
                fundBookReport.TotalThu = query.Where(x => x.Type == "inbound").Sum(x => x.AmountSigned);
                fundBookReport.TotalChi = query.Where(x => x.Type == "outbound").Sum(x => x.AmountSigned);
            }

            //return total = begin + thu - chi
            fundBookReport.TotalAmount = fundBookReport.Begin + fundBookReport.TotalThu + fundBookReport.TotalChi;

            return fundBookReport;
        }

        public async Task<List<FundBookExportExcel>> GetExportExcel(VFundBookSearch val)
        {
            var query = _FilterQueryable(val);
            var totalItems = await query.CountAsync();
            query = query.Where(x => x.State == "posted").OrderByDescending(x => x.Date).Skip(val.Offset).Take(val.Limit);
            var items = await query.Include(x => x.Journal).ToListAsync();

            var res = items.Select(x => new FundBookExportExcel
            {
                Date = x.Date,
                Name = x.Name,
                Type = x.Type,
                Type2 = x.Type2,
                Amount = x.Amount,
                RecipientPayer = x.RecipientPayer,
                State = x.State
            }).ToList();

            return res;
        }
    }
}
