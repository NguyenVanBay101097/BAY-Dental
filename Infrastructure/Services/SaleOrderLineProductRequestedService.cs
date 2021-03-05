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
    public class SaleOrderLineProductRequestedService: BaseService<SaleOrderLineProductRequested>, ISaleOrderLineProductRequestedService
    {
        private readonly IMapper _mapper;

        public SaleOrderLineProductRequestedService(
            IAsyncRepository<SaleOrderLineProductRequested> repository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SaleOrderLineProductRequestedBasic>> GetPagedResultAsync(SaleOrderLineProductRequestedPaged val)
        {
            var query = SearchQuery();

            if (val.SaleOrderLineId.HasValue)
            {
                query = query.Where(x => x.SaleOrderLineId == val.SaleOrderLineId);
            }

            if (val.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == val.ProductId);
            }

            var count = await query.CountAsync();

            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var item = await _mapper.ProjectTo<SaleOrderLineProductRequestedBasic>(query).ToListAsync();


            return new PagedResult2<SaleOrderLineProductRequestedBasic>(count, val.Offset, val.Limit) {
                Items = item
            };
        }
    }
}
