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
    public class PartnerSourceService : BaseService<PartnerSource>, IPartnerSourceService
    {
        private readonly IMapper _mapper;
        public PartnerSourceService(IAsyncRepository<PartnerSource> repository, IHttpContextAccessor httpContextAccessor , IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<PartnerSourceBasic>> GetPagedResultAsync(PartnerSourcePaged val)
        {
            ISpecification<PartnerSource> spec = new InitialSpecification<PartnerSource>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<PartnerSource>(x => x.Name.Contains(val.Search)));
            if (!string.IsNullOrEmpty(val.Type))
                spec = spec.And(new InitialSpecification<PartnerSource>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<PartnerSourceBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<PartnerSourceBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        //public async Task<PartnerSourceBasic> CreateAsync(PartnerSourceSave val)
        //{
        //    var res = _mapper.Map<PartnerSource>(val);

        //    await CreateAsync(res);

        //    var basic = _mapper.Map<PartnerSourceBasic>(res);
        //    return basic;
        //}

        //public async Task UpdatePartnerResource(PartnerSourceSave val)
        //{
        //    var res = _mapper.Map<PartnerSource>(val);

        //    await UpdateAsync(res);
        //}

       
    }
}
