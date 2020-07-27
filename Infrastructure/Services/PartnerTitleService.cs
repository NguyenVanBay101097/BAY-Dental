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

        public PartnerTitleService(
            IAsyncRepository<PartnerTitle> repository, 
            IHttpContextAccessor httpContextAccessor, 
            IMapper mapper
        ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
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

        public override async Task<IEnumerable<PartnerTitle>> CreateAsync(IEnumerable<PartnerTitle> entities)
        {
            await base.CreateAsync(entities);
            return entities;
        }

        public override async Task UpdateAsync(IEnumerable<PartnerTitle> entities)
        {

            await base.UpdateAsync(entities);
        }
    }
}
