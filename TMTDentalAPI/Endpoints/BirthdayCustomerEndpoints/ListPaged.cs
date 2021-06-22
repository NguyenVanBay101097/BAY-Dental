using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Endpoints.BirthdayCustomerEndpoints
{
    public class ListPaged : BaseAsyncEndpoint
        .WithRequest<ListPagedBirthdayCustomerRequest>
        .WithResponse<ListPagedBirthdayCustomerResponse>
    {
        private readonly IPartnerService _partnerService;
        private readonly IAsyncRepository<SaleReport> _saleReportRepository;

        public ListPaged(IPartnerService partnerService,
            IAsyncRepository<SaleReport> saleReportRepository)
        {
            _partnerService = partnerService;
            _saleReportRepository = saleReportRepository;
        }

        protected Guid CompanyId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }

        [HttpGet("api/BirthdayCustomers")]
        public override async Task<ActionResult<ListPagedBirthdayCustomerResponse>> HandleAsync([FromQuery]ListPagedBirthdayCustomerRequest request, CancellationToken cancellationToken = default)
        {
            var companyId = CompanyId;
            var saleReportQuery = _saleReportRepository.SearchQuery(x => x.State == "sale" || x.State == "done");
            if (companyId != Guid.Empty)
                saleReportQuery = saleReportQuery.Where(x => x.CompanyId == companyId);

            var partnerSaleReport = from sale_report in saleReportQuery
                         group sale_report by sale_report.PartnerId into g
                         select new
                         {
                             PartnerId = g.Key,
                             PriceTotal = g.Sum(x => x.PriceTotal),
                             TotalQty = g.Sum(x => x.ProductUOMQty)
                         };

            var today = DateTime.Today;
            var partnerQuery = _partnerService.SearchQuery(x => x.Customer == true && x.BirthDay == today.Day && x.BirthMonth == today.Month);
            if (!string.IsNullOrEmpty(request.Search))
                partnerQuery = partnerQuery.Where(x => x.Name.Contains(request.Search) || x.NameNoSign.Contains(request.Search) || x.Phone.Contains(request.Search));

            var query = from partner in partnerQuery
                        from sale_report in partnerSaleReport.Where(x => x.PartnerId == partner.Id).DefaultIfEmpty()
                        select new BirthdayCustomerDto
                        {
                            Id = partner.Id,
                            Name = partner.Name,
                            PriceTotal = sale_report.PriceTotal,
                            TotalQty = sale_report.TotalQty,
                            Phone = partner.Phone,
                            BirthDay = partner.BirthDay,
                            BirthMonth = partner.BirthMonth,
                            BirthYear = partner.BirthYear,
                            Gender = partner.Gender
                        };


            var totalItems = await query.CountAsync();
            query = query.OrderBy(x => x.Name).Skip(request.Offset);
            if (request.Limit > 0)
                query = query.Take(request.Limit);
            var items = await query.ToListAsync();

            return new ListPagedBirthdayCustomerResponse()
            {
                TotalItems = totalItems,
                Items = items
            };
        }
    }
}
