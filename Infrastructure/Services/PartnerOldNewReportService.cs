using ApplicationCore.Entities;
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
    public class PartnerOldNewReportService : IPartnerOldNewReportService
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PartnerOldNewReportService(CatalogDbContext context, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<PartnerOldNewReport> _GetQueryAble(PartnerOldNewReportSearch val)
        {
            var userObj = GetService<IUserService>();
            var company_ids = userObj.GetListCompanyIdsAllowCurrentUser();
            var query = _context.PartnerOldNewReports.Where(x => company_ids.Contains(x.CompanyId));

            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == val.CompanyId.Value);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom.Value);

            if (val.DateTo.HasValue)
                query = query.Where(x => x.Date <= val.DateTo.Value);
            return query;
        }

        public async Task<PartnerOldNewReportVM> GetSumaryPartnerOldNewReport(PartnerOldNewReportSearch val)
        {
            var query = _GetQueryAble(val);
            var partners = query.AsEnumerable().GroupBy(x => x.PartnerId).Select(x => new
            {
                PartnerId = x.Key,
                LastDateOrder = x.Max(s => s.Date),
                PartnerOldNew = x.LastOrDefault(),
            }).ToList();

            var result = new PartnerOldNewReportVM()
            {
                TotalNewPartner = partners.Where(x => x.PartnerOldNew.Type == "KHM").Count(),
                TotalOldPartner = partners.Where(x => x.PartnerOldNew.Type == "KHC").Count(),
            };

            return result;
        }

        public async Task<IEnumerable<PartnerOldNewReportVM>> GetPartnerOldNewReport(PartnerOldNewReportSearch val)
        {
            var query = _GetQueryAble(val);

            var result = query.AsEnumerable().GroupBy(x => new
            {
                Year = x.Date.Year,
                WeekOfYear = GetIso8601WeekOfYear(x.Date)
            })
                  .Select(x => new PartnerOldNewReportVM
                  {
                      WeekOfYear = x.Key.WeekOfYear,
                      Year = x.Key.Year,
                      TotalNewPartner = x.Distinct().Where(x => x.Type == "KHM").Count(),
                      TotalOldPartner = x.Distinct().Where(x => x.Type == "KHC").Count(),
                      OrderLines = _mapper.Map<IEnumerable<PartnerOldNewReportVMDetail>>(x.ToList()),
                  }).ToList();
            return result;
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
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

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }
    }
}
