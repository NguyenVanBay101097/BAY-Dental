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
using ApplicationCore.Utilities;
using Dapper;

namespace Infrastructure.Services
{
    public class CashBookService : ICashBookService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashBookService(CatalogDbContext context, IMapper mapper,
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

        public async Task<CashBookReport> GetSumary(CashBookSearch val)
        {
            var amlObj = GetService<IAccountMoveLineService>();
            var cashBookReport = new CashBookReport();

            var types = new string[] { "cash", "bank" };
            if (val.ResultSelection == "cash")
                types = new string[] { "cash" };
            else if (val.ResultSelection == "bank")
                types = new string[] { "bank" };

            var dateFrom = val.DateFrom;
            if (dateFrom.HasValue)
                dateFrom = dateFrom.Value.AbsoluteBeginOfDate();

            var dateTo = val.DateTo;
            if (dateTo.HasValue)
                dateTo = dateTo.Value.AbsoluteEndOfDate();

            if (dateFrom.HasValue)
            {
                var query1 = amlObj._QueryGet(dateFrom: dateFrom, state: "posted", companyId: val.CompanyId, initBal: true);
                query1 = query1.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType == "liquidity");
                var begin = await query1.SumAsync(x => x.Debit - x.Credit);
                cashBookReport.Begin = begin;
            }

            var query = amlObj._QueryGet(dateFrom: dateFrom, dateTo: dateTo, state: "posted", companyId: val.CompanyId);
            query = query.Where(x => types.Contains(x.Journal.Type) && x.AccountInternalType == "liquidity");
            cashBookReport.TotalThu = await query.SumAsync(x => x.Debit);
            cashBookReport.TotalChi = await query.SumAsync(x => x.Credit);

            cashBookReport.TotalAmount = cashBookReport.Begin + cashBookReport.TotalThu - cashBookReport.TotalChi;
            return cashBookReport;
        }

        private bool checkExistsTable(string tableName)
        {
            var sqlQ = $"SELECT COUNT(*) as Count FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
            var conn = _context.Database.GetDbConnection();
            {
                if (conn != null)
                {
                    // Query - method extension provided by Dapper library
                    bool exists = conn.Query<int>(sqlQ).FirstOrDefault() > 0;
                    return exists;
                }
            }
            return false;
        }

        public async Task ChangeData()
        {
            if (checkExistsTable("SalaryPayments"))
            {

            }
            if (checkExistsTable("PhieuThuChis"))
            {

            }
        }
    }
}
