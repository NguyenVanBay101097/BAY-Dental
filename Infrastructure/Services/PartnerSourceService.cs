using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class PartnerSourceService : BaseService<PartnerSource>, IPartnerSourceService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        public PartnerSourceService(IAsyncRepository<PartnerSource> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper , CatalogDbContext context)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedResult2<PartnerSourceBasic>> GetPagedResultAsync(PartnerSourcePaged val)
        {
            ISpecification<PartnerSource> spec = new InitialSpecification<PartnerSource>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PartnerSource>(x => x.Name.Contains(val.Search)));
            //if (!string.IsNullOrEmpty(val.Type))
            //    spec = spec.And(new InitialSpecification<PartnerSource>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<PartnerSourceBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<PartnerSourceBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<PartnerSourceBasic>> GetAutocompleteAsync(PartnerSourcePaged val)
        {
            ISpecification<PartnerSource> spec = new InitialSpecification<PartnerSource>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PartnerSource>(x => x.Name.Contains(val.Search)));
            //if (!string.IsNullOrEmpty(val.Type))
            //    spec = spec.And(new InitialSpecification<PartnerSource>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<PartnerSourceBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();

            return items;
        }

        public override async Task<IEnumerable<PartnerSource>> CreateAsync(IEnumerable<PartnerSource> entities)
        {

            await base.CreateAsync(entities);
            await CheckType();
            return entities;
        }

        public override async Task UpdateAsync(IEnumerable<PartnerSource> entities)
        {

            await base.UpdateAsync(entities);
            await CheckType();
        }

        public async Task CheckType()
        {
            //chi cho phep co 1 nguon la referral neu co hon 1 thi bao loi
            var res = await SearchQuery(x => x.Type == "referral").ToListAsync();
            if (res.Count() > 1)
            {
                throw new Exception("Kiểu giới thiệu đã tồn tại");
            }
        }

        public async Task<List<ReportSource>> GetReportPartnerSource(ReportFilterpartnerSource val)
        {
            var companyId = CompanyId;
            var pt = new List<Partner>();
            var partnerObj = GetService<IPartnerService>();
            var partners = await partnerObj.SearchQuery(x => x.CompanyId == companyId).ToListAsync();
            var query = await SearchQuery().Select(x => new ReportSource {
                Id = x.Id,
                Name = x.Name,
                CountPartner = _context.Partners.Where(s => s.SourceId == x.Id && s.CompanyId == companyId).Count()
            }).ToListAsync();
            if (val.SourceId.HasValue)
                query = query.Where(x => x.Id == val.SourceId.Value).ToList();

            return query;
        }
    }

    public class ReportFilterpartnerSource
    {
        //loc theo nguồn
        public Guid? SourceId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string Search { get; set; }
        /// <summary>
        /// week : tuần này
        /// month : tháng này
        /// year : năm này
        /// </summary>
        public string Groupby { get; set; }
    }

    public class ReportSource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CountPartner { get; set; }
    }

}
