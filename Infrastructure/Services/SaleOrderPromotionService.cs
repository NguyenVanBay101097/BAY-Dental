using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class SaleOrderPromotionService : BaseService<SaleOrderPromotion> , ISaleOrderPromotionService
    {
        private readonly IMapper _mapper;

        public SaleOrderPromotionService(IAsyncRepository<SaleOrderPromotion> repository, IHttpContextAccessor httpContextAccessor,IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SaleOrderPromotionDisplay>> GetPagedResultAsync(SaleOrderPromotionPaged val)
        {
            var query = SearchQuery();

            if (val.SaleOrderId.HasValue)
                query = query.Where(x => x.SaleOrderId == val.SaleOrderId.Value);

            if (val.SaleOrderLineId.HasValue)
                query = query.Where(x => x.SaleOrderLineId == val.SaleOrderLineId.Value && !x.ParentId.HasValue);

            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.Include(x => x.SaleOrderPromotionChilds).ToListAsync();

            var paged = new PagedResult2<SaleOrderPromotionDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SaleOrderPromotionDisplay>>(items)
            };

            return paged;
        }


    }
}
