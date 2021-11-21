using ApplicationCore.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<ImsuranceDebtReport>> GetInsuranceDebtReport(ImsuranceDebtFilter val)
        {
            var accMoveObj = GetService<IAccountMoveService>();
            var insuranceObj = GetService<IResInsuranceService>();
            var accPaymentObj = GetService<IAccountPaymentService>();
           

            var insurance = await insuranceObj.GetByIdAsync(val.InsuranceId);
            var invoiceIds = await accPaymentObj.SearchQuery(x => x.State == "posted" && x.MoveLines.Any(s => s.PartnerId == insurance.PartnerId && s.Account.Code == "CNBH")).SelectMany(x => x.AccountMovePaymentRels).Select(x => x.MoveId).Distinct().ToListAsync();

            var query = accMoveObj.SearchQuery(x => invoiceIds.Contains(x.Id) && x.Type == "out_invoice"  && x.InvoicePaymentState == "paid");
            //var types = new[] { "receivable"};
            //query = query.Where(x => x.PartnerId == insurance.PartnerId && x.Reconciled == false && x.PaymentId.HasValue);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.InvoiceOrigin.Contains(val.Search) || x.Partner.Name.Contains(val.Search));

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.Date).Select(x => new ImsuranceDebtReport
            {
                Date = x.Date,
                PartnerName = x.Partner.Name,
                AmountTotal =  x.AmountTotal ?? 0,
                Origin = x.InvoiceOrigin,
                MoveId = x.Id,
                MoveType = x.Type
            }).ToListAsync();

            return items;
        }
    }
}
