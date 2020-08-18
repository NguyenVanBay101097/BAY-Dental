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
    public class PartnerSourceService : BaseService<PartnerSource>, IPartnerSourceService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        public PartnerSourceService(IAsyncRepository<PartnerSource> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, CatalogDbContext context)
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

        public async Task<List<ReportPartnerSourceItem>> GetReportPartnerSource(ReportFilterPartnerSource val)
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
            }).Select(x => new ReportPartnerSourceItem
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

        public async Task InsertModelsIfNotExists()
        {
            var modelObj = GetService<IIRModelService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var model = await modelDataObj.GetRef<IRModel>("base.model_res_partner_source");
            if (model == null)
            {
                model = new IRModel
                {
                    Name = "Nguồn khách hàng",
                    Model = "PartnerSource",
                };

                modelObj.Sudo = true;
                await modelObj.CreateAsync(model);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "model_res_partner_source",
                    Module = "base",
                    Model = "ir.model",
                    ResId = model.Id.ToString()
                });
            }
        }
    }

    public class ReportFilterPartnerSource
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class ReportPartnerSourceItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int TotalPartner { get; set; }
        public int CountPartner { get; set; }
    }


}
