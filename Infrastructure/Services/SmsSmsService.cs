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
    public class SmsSmsService : BaseService<SmsSms>, ISmsSmsService
    {
        private readonly IMapper _mapper;

        public SmsSmsService(IAsyncRepository<SmsSms> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SmsSmsBasic>> GetPaged(SmsSmsPaged val)
        {
            var query = SearchQuery();
            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId.Value);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Number.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Equals(val.State));
            var totalItems = await query.CountAsync();
            var items = await query.Include(x => x.Partner).Take(val.Limit).Skip(val.Offset).ToListAsync();
            return new PagedResult2<SmsSmsBasic>(totalItems: totalItems, limit: val.Limit, offset: val.Offset)
            {
                Items = _mapper.Map<IEnumerable<SmsSmsBasic>>(items)
            };
        }
    }
}
