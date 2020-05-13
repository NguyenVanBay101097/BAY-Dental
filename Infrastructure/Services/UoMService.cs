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
    public class UoMService : BaseService<UoM>, IUoMService
    {

        private readonly IMapper _mapper;
        public UoMService(IAsyncRepository<UoM> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public override Task<IEnumerable<UoM>> CreateAsync(IEnumerable<UoM> entities)
        {
            CheckRoundingAndFactor(entities);
            return base.CreateAsync(entities);
        }

        public override Task UpdateAsync(IEnumerable<UoM> entities)
        {
            CheckRoundingAndFactor(entities);
            return base.UpdateAsync(entities);
        }

        public void CheckRoundingAndFactor(IEnumerable<UoM> self)
        {
            foreach (var uom in self)
            {
                if (uom.Rounding <= 0)
                    throw new Exception("Thuộc tính 'làm tròn' phải lớn hơn không");

                if (uom.Factor == 0)
                    throw new Exception("Thuộc tính 'tỉ lệ' phải khác không");
            }
        }

        public async Task<UoM> DefaultUOM()
        {
            var res = await SearchQuery().FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<UoMBasic>> GetPagedResultAsync(UoMPaged val)
        {
            ISpecification<UoM> spec = new InitialSpecification<UoM>(x => x.Active == true);
            if (!string.IsNullOrEmpty(val.Search))
            {
                spec = spec.And(new InitialSpecification<UoM>(x => x.Name.Contains(val.Search)));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<UoMBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<UoMBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<UoMBasic>> GetAutocompleteAsync(UoMPaged val)
        {
            var query = GetQueryPaged(val);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (val.CategId.HasValue)
            {
                var uom = await GetByIdAsync(val.CategId.Value);
                query = query.Where(x => x.CategoryId == uom.CategoryId);
            }

            if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductUoMRels.Any(s=>s.ProductId == val.ProductId.Value));
            }

            var items = await _mapper.ProjectTo<UoMBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();

            return items;
        }


        private IQueryable<UoM> GetQueryPaged(UoMPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            query = query.OrderBy(s => s.DateCreated);
            return query;
        }
    }
}
