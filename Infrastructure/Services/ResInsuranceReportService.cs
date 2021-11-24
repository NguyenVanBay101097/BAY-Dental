using ApplicationCore.Entities;
using ApplicationCore.Models;
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
                query = query.Where(x => x.PaymentId.HasValue && (x.Payment.Communication.Contains(val.Search) || x.Payment.Partner.Name.Contains(val.Search) || x.Payment.Partner.NameNoSign.Contains(val.Search)));

            var items = await query.OrderByDescending(x => x.DateCreated).Select(x => new InsuranceDebtReport
            {
                Date = x.DateCreated,
                PartnerName = x.Payment.Partner.Name,
                AmountTotal = x.AmountResidual,
                Origin = x.Move.InvoiceOrigin,
                Communication = x.Payment.Communication,
                MoveId = x.Move.Id,
                MoveType = x.Move.Type
            }).ToListAsync();

            return items;
        }

        public async Task<PagedResult2<InsuranceHistoryInComeItem>> GetHistoryInComeDebtPaged(InsuranceHistoryInComeFilter val)
        {
            var paymentObj = GetService<IAccountPaymentService>();
            var insuranceObj = GetService<IResInsuranceService>();
            var paymentQr = paymentObj.SearchQuery();

            if (val.InsuranceId.HasValue)
            {
                var insurance = await insuranceObj.GetByIdAsync(val.InsuranceId);
                paymentQr = paymentQr.Where(x => x.PartnerId == insurance.PartnerId);
            }
               
            if (val.DateFrom.HasValue)
                paymentQr = paymentQr.Where(x => x.PaymentDate >= val.DateFrom.Value.AbsoluteBeginOfDate());

            if (val.DateTo.HasValue)
                paymentQr = paymentQr.Where(x => x.PaymentDate < val.DateTo.Value.AbsoluteEndOfDate());

            paymentQr = paymentQr.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                paymentQr = paymentQr.Skip(val.Offset).Take(val.Limit);

            var totalItems = await paymentQr.CountAsync();



            var items = await paymentQr.Select(x => new InsuranceHistoryInComeItem
            {
                Id = x.Id,
                PaymentDate = x.PaymentDate,
                Name = x.Name,
                Amount = x.Amount,
                Communication = x.Communication,
                JournalName = x.Journal.Name,
                State = x.State,
                DateFrom = val.DateFrom,
                DateTo = val.DateTo
            }).ToListAsync();

            return new PagedResult2<InsuranceHistoryInComeItem>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };

        }

        public async Task<IEnumerable<InsuranceHistoryInComeDetailItem>> GetHistoryInComeDebtDetails(InsuranceHistoryInComeDetailFilter val)
        {
            var partialReconcileObj = GetService<IAccountPartialReconcileService>();
            var insuranceObj = GetService<IResInsuranceService>();
            var query = partialReconcileObj.SearchQuery();

            if (val.PaymentId.HasValue)
                query = query.Where(x => x.CreditMove.PaymentId == val.PaymentId);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateCreated >= val.DateFrom.Value.AbsoluteBeginOfDate());

            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateCreated < val.DateTo.Value.AbsoluteEndOfDate());

            query = query.OrderByDescending(x => x.DateCreated);         

            var items = await query.Select(x => new InsuranceHistoryInComeDetailItem
            {
                Id = x.Id,
                PartnerName = x.DebitMove.Partner.Name,
                Amount = x.Amount,
                Date = x.DateCreated.Value,
                Ref = x.DebitMove.Ref
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
            var insuranceObj = GetService<IResInsuranceService>();

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
            query2 = query2.Where(x => x.PartnerId.HasValue && x.Partner.IsInsurance && x.Account.Code == "CNBH");

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


            var partnerIds = dict.Select(x => x.Key).ToList();
            var dict_insurance = insuranceObj.SearchQuery(x => x.PartnerId.HasValue && partnerIds.Contains(x.PartnerId.Value)).Distinct().ToDictionary(x => x.PartnerId, x => x.Id);
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
                    InsuranceId = dict_insurance[item.Key],
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
                        PaymentName = x.Payment.Name,
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

        public async Task<IEnumerable<ReportInsuranceDebitExcel>> ExportReportInsuranceDebtExcel(InsuranceReportFilter val)
        {
            var reportDebitDetailFilter = new InsuranceReportDetailFilter()
            {
                DateFrom = val.DateFrom,
                DateTo = val.DateTo,
                CompanyId = val.CompanyId,
            };

            var data = _mapper.Map<IEnumerable<ReportInsuranceDebitExcel>>(await ReportSummary(val));
            foreach (var line in data)
            {

                reportDebitDetailFilter.PartnerId = line.PartnerId;
                line.Lines = await ReportDetail(reportDebitDetailFilter);
            }

            return data;
        }

    }
}
