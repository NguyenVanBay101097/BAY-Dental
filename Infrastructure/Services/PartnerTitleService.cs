using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
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
    public class PartnerTitleService : BaseService<PartnerTitle>, IPartnerTitleService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        public PartnerTitleService(
            IAsyncRepository<PartnerTitle> repository, 
            IHttpContextAccessor httpContextAccessor, 
            IMapper mapper, 
            CatalogDbContext context
        ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedResult2<PartnerTitleBasic>> GetPagedResultAsync(PartnerTitlePaged val)
        {
            ISpecification<PartnerTitle> spec = new InitialSpecification<PartnerTitle>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PartnerTitle>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<PartnerTitleBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<PartnerTitleBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<PartnerTitleBasic>> GetAutocompleteAsync(PartnerTitlePaged val)
        {
            ISpecification<PartnerTitle> spec = new InitialSpecification<PartnerTitle>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PartnerTitle>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<PartnerTitleBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();

            return items;
        }

        public override async Task<IEnumerable<PartnerTitle>> CreateAsync(IEnumerable<PartnerTitle> entities)
        {
            await base.CreateAsync(entities);
            return entities;
        }

        public async Task<List<ReportPartnerTitleItem>> GetReportPartnerTitle(ReportFilterPartnerTitle val)
        {
            var companyId = CompanyId;
            //SearchQuery
            var partnerObj = GetService<IPartnerService>();
            var partners = partnerObj.SearchQuery(x => x.Customer == true);

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                partners = partners.Where(x => x.Date.Value >= dateFrom);
            }
            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                partners = partners.Where(x => x.Date.Value <= dateTo);
            }
            var totalPartner = partners.ToList().Count;

            var query = partners.GroupBy(x => new
            {
                x.Source.Id,
                x.Source.Name
            }).Select(x => new ReportPartnerTitleItem
            {
                Id = x.Key.Id,
                Name = x.Key.Name,
                TotalPartner = totalPartner,
                CountPartner = x.Count()
            });

            var list = await query.ToListAsync();
            list.Sum(x => x.CountPartner);
            return list;
        }
    }
}
