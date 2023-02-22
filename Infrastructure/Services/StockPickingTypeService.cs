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
    public class StockPickingTypeService : BaseService<StockPickingType>, IStockPickingTypeService
    {
        private readonly IMapper _mapper;

        public StockPickingTypeService(IAsyncRepository<StockPickingType> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<StockPickingTypeBasic>> GetPagedResultAsync(StockPickingTypePaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<StockPickingTypeBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<StockPickingTypeBasic>>(items)
            };
        }

        private IQueryable<StockPickingType> GetQueryPaged(StockPickingTypePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Warehouse.Name.Contains(val.Search));

            query = query.OrderBy(s => s.Sequence);
            return query;
        }

        public override ISpecification<StockPickingType> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "stock.stock_picking_type_rule":
                    return new InitialSpecification<StockPickingType>(x => !x.WarehouseId.HasValue || x.Warehouse.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
