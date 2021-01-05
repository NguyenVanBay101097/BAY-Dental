using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
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
    public class MedicineOrderService : BaseService<MedicineOrder>, IMedicineOrderService
    {
        private readonly IMapper _mapper;
        public MedicineOrderService(IAsyncRepository<MedicineOrder> repository, IHttpContextAccessor httpContextAccessor, IMapper mappner)
            : base(repository, httpContextAccessor)
        {
            _mapper = mappner;
        }


        public async Task<PagedResult2<MedicineOrderBasic>> GetPagedResultAsync(MedicineOrderPaged val)
        {
            var query = SearchQuery(x => x.CompanyId == CompanyId);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) ||
                   x.Partner.Name.Contains(val.Search));

            if (string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.OrderDate >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.OrderDate <= dateOrderTo);
            }


            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await _mapper.ProjectTo<MedicineOrderBasic>(query.OrderByDescending(x => x.DateCreated)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<MedicineOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
