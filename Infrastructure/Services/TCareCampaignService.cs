using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
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
    public class TCareCampaignService : BaseService<TCareCampaign>, ITCareCampaignService
    {
        private readonly IMapper _mapper;

        public TCareCampaignService(IAsyncRepository<TCareCampaign> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<TCareCampaignBasic>> GetPagedResultAsync(TCareCampaignPaged val)
        {
            ISpecification<TCareCampaign> spec = new InitialSpecification<TCareCampaign>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<TCareCampaign>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<TCareCampaignBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<TCareCampaignBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<TCareCampaign> NameCreate(TCareCampaignNameCreateVM val)
        {
            var campaign = new TCareCampaign() { Name = val.Name };
            return await CreateAsync(campaign);
        }
    }
}
