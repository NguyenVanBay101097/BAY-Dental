using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ResInsuranceReportService : IResInsuranceReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResInsuranceReportService(IHttpContextAccessor httpContextAccessor, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
        }

        public T GetService<T>()
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

        private string UserId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return null;

                return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }

        public async Task<IEnumerable<InsuranceDebtReport>> GetInsuranceDebtReport(InsuranceDebtFilter val)
        {
            var moveLineObj = GetService<IAccountMoveLineService>();
            var insuranceObj = GetService<IResInsuranceService>();

            var insurance = await insuranceObj.GetByIdAsync(val.InsuranceId);
            var query = moveLineObj.SearchQuery(x => x.PartnerId == insurance.PartnerId && x.Journal.Type == "insurance" && x.AmountResidual > 0);          
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Move.InvoiceOrigin.Contains(val.Search) || (x.PaymentId.HasValue && x.Payment.Communication.Contains(val.Search)));

            var items = await query.OrderByDescending(x => x.DateCreated).Select(x => new InsuranceDebtReport
            {
                Date = x.DateCreated,
                PartnerName = x.PartnerId.HasValue ? x.Payment.Partner.Name : null,
                AmountTotal = x.AmountResidual,
                Origin = x.Move.InvoiceOrigin,
                Communication = x.PartnerId.HasValue ? x.Payment.Communication : null,
                MoveId = x.Move.Id,
                MoveType = x.Move.Type
            }).ToListAsync();

            return items;
        }

        public async Task<IEnumerable<InsuranceReportItem>> ReportSummary(InsuranceReportFilter val)
        {
            var today = DateTime.Today;
            var date_from = val.DateFrom;
            var date_to = val.DateTo;
            if (date_to.HasValue)
                date_to = date_to.Value.AbsoluteEndOfDate();

            var amlObj = GetService<IAccountMoveLineService>();

            var dict = new Dictionary<Guid, InsuranceReportItem>();
            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, state: "posted", companyId: val.CompanyId);
                query = query.Where(x => x.PartnerId.HasValue && x.Partner.IsInsurance && x.Account.Code == "CNBH");

                if (!string.IsNullOrWhiteSpace(val.Search))
                {
                    query = query.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                    x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
                }

                var list = await query
                   .GroupBy(x => new
                   {
                       PartnerId = x.Partner.Id,
                       PartnerName = x.Partner.Name,
                       PartnerRef = x.Partner.Ref,
                       PartnerPhone = x.Partner.Phone,
                       Type = x.Account.InternalType
                   })
                   .Select(x => new
                   {
                       PartnerId = x.Key.PartnerId,
                       PartnerName = x.Key.PartnerName,
                       PartnerRef = x.Key.PartnerRef,
                       PartnerPhone = x.Key.PartnerPhone,
                       x.Key.Type,
                       InitialBalance = x.Sum(s => s.Debit - s.Credit),
                   }).ToListAsync();

                foreach (var item in list)
                {
                    if (!dict.ContainsKey(item.PartnerId))
                    {
                        dict.Add(item.PartnerId, new InsuranceReportItem()
                        {
                            PartnerId = item.PartnerId,
                            PartnerName = item.PartnerName,
                            PartnerRef = item.PartnerRef,
                            PartnerPhone = item.PartnerPhone,
                            DateFrom = date_from,
                            DateTo = date_to
                        });
                    }

                    dict[item.PartnerId].Begin = item.InitialBalance;
                }
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted", companyId: val.CompanyId);
            query2 = query2.Where(x => x.PartnerId.HasValue &&  x.Partner.IsInsurance && x.Account.Code == "CNBH");

            if (!string.IsNullOrWhiteSpace(val.Search))
            {
                query2 = query2.Where(x => x.Partner.Name.Contains(val.Search) || x.Partner.NameNoSign.Contains(val.Search) ||
                x.Partner.Phone.Contains(val.Search) || x.Partner.Ref.Contains(val.Search));
            }

            var list2 = await query2
                      .GroupBy(x => new
                      {
                          PartnerId = x.Partner.Id,
                          PartnerName = x.Partner.Name,
                          PartnerRef = x.Partner.Ref,
                          PartnerPhone = x.Partner.Phone,
                          Type = x.Account.InternalType
                      })
                    .Select(x => new
                    {
                        PartnerId = x.Key.PartnerId,
                        PartnerName = x.Key.PartnerName,
                        PartnerRef = x.Key.PartnerRef,
                        PartnerPhone = x.Key.PartnerPhone,
                        x.Key.Type,
                        Debit = x.Sum(s => s.Debit),
                        Credit = x.Sum(s => s.Credit),
                    }).ToListAsync();

            foreach (var item in list2)
            {
                if (!dict.ContainsKey(item.PartnerId))
                {
                    dict.Add(item.PartnerId, new InsuranceReportItem()
                    {
                        PartnerId = item.PartnerId,
                        PartnerName = item.PartnerName,
                        PartnerRef = item.PartnerRef,
                        PartnerPhone = item.PartnerPhone,
                        DateFrom = date_from,
                        DateTo = date_to
                    });
                }

                dict[item.PartnerId].Debit = item.Debit;
                dict[item.PartnerId].Credit = item.Credit;           
            }

            var res = new List<InsuranceReportItem>();
            foreach (var item in dict)
            {
                var begin = dict[item.Key].Begin;
                var debit = dict[item.Key].Debit;
                var credit = dict[item.Key].Credit;
                var end = begin + debit - credit;
                var value = item.Value;
                res.Add(new InsuranceReportItem
                {
                    PartnerId = item.Key,
                    DateFrom = date_from,
                    DateTo = date_to,
                    PartnerRef = value.PartnerRef,
                    Begin = begin,
                    Debit = debit,
                    Credit = credit,
                    End = end,
                    PartnerName = value.PartnerName,
                    PartnerPhone = value.PartnerPhone,
                });
            }
            return res;
        }

        public async Task<IEnumerable<InsuranceReportDetailItem>> ReportDetail(InsuranceReportDetailFilter val)
        {
            var date_from = val.DateFrom;
            var date_to = val.DateTo;
            var amlObj = GetService<IAccountMoveLineService>();

            var sign = 1;
            decimal begin = 0;
            var res = new List<InsuranceReportDetailItem>();

            if (date_from.HasValue)
            {
                var query = amlObj._QueryGet(dateFrom: date_from, dateTo: null, initBal: true, state: "posted");
                query = query.Where(x => x.PartnerId == val.PartnerId && x.Partner.IsInsurance && x.Account.Code == "CNBH");
                begin = (await query.SumAsync(x => x.Debit - x.Credit)) * sign;
            }

            var query2 = amlObj._QueryGet(dateFrom: date_from, dateTo: date_to, state: "posted");
            query2 = query2.Where(x => x.PartnerId == val.PartnerId && x.Partner.IsInsurance && x.Account.Code == "CNBH");
            var list2 = query2.OrderBy(x => x.DateCreated)
                    .Select(x => new InsuranceReportDetailItem
                    {
                        Date = x.Date,
                        MoveName = x.Move.Name,
                        Name = x.Name,
                        Ref = x.Move.Ref,
                        Debit = x.Debit,
                        Credit = x.Credit,
                    }).ToList();


            foreach (var item in list2)
            {
                item.Begin = begin;
                item.End = item.Begin + item.Debit - item.Credit;
                begin = item.End;
            }

            return list2;
        }

        public async Task<InsuranceReportPrint> ReportSummaryPrint(InsuranceReportFilter val)
        {
            var data = await ReportSummary(val);
            var res = new InsuranceReportPrint()
            {
                DateFrom = val.DateFrom,
                DateTo = val.DateTo,
                Data = data,
            };
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            res.User = _mapper.Map<ApplicationUserSimple>(user);

            if (val.CompanyId.HasValue)
            {
                var companyObj = GetService<ICompanyService>();
                res.Company = _mapper.Map<CompanyPrintVM>(await companyObj.SearchQuery(x => x.Id == val.CompanyId)
                    .Include(x => x.Partner).FirstOrDefaultAsync());
            }
            return res;
        }

    }
}
