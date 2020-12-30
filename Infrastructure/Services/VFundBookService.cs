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

            if (!val.InitBal && val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);

            if (val.InitBal && val.DateFrom.HasValue)
                query = query.Where(x => x.Date < val.DateFrom.Value);

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
            query = query.Take(val.Limit).Skip(val.Offset);
            var items = await query.Include(x => x.Journal).ToListAsync();

            return new PagedResult2<VFundBookDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<VFundBookDisplay>>(items)
            };
        }

        public async Task<FundBookSumary> GetSumary()
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var query = _context.VFundBooks.Where(x => company_ids.Contains(x.CompanyId));
            var list = await query.Include(x => x.Journal).ToListAsync();
            var fundBookSumary = new FundBookSumary();
            if (list.Any())
            {
                fundBookSumary.TotalBank = list.Where(x => x.Journal.Type == "bank" && (x.State == "done" || x.State == "posted")).Sum(x => x.Amount);
                fundBookSumary.TotalCash = list.Where(x => x.Journal.Type == "cash" && (x.State == "done" || x.State == "posted")).Sum(x => x.Amount);
                fundBookSumary.TotalAmount = list.Where(x => x.State == "done" || x.State == "posted").Sum(x => x.Amount);
            }
            return fundBookSumary;
        }

        public async Task<FundBookReport> GetReport(VFundBookSearch val)
        {
            var query = _FilterQueryable(val);
            var list = await query.ToListAsync();
            var fundBookReport = new FundBookReport();
            if (list.Any())
            {
                fundBookReport.TotalChi = list.Where(x => x.Type == "outbound").Sum(x => x.Amount);
                fundBookReport.TotalThu = list.Where(x => x.Type == "inbound").Sum(x => x.Amount);
            }

            val.InitBal = true;
            IQueryable<VFundBook> query2 = _FilterQueryable(val);
            var list2 = await query2.ToListAsync();
            if (list2.Any())
            {
                var totalThu = list2.Where(x => x.Type == "inbound").Sum(x => x.Amount);
                var totalChi = list2.Where(x => x.Type == "outbound").Sum(x => x.Amount);
                fundBookReport.Begin = totalThu - totalChi;
                fundBookReport.TotalAmount = fundBookReport.Begin + fundBookReport.TotalThu - fundBookReport.TotalChi;

            }
            else
            {
                fundBookReport.TotalAmount = fundBookReport.Begin + fundBookReport.TotalThu - fundBookReport.TotalChi;
                fundBookReport.Begin = 0;
            }


            return fundBookReport;
        }
    }
}
