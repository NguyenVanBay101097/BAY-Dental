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
    public class SmsMessageService : BaseService<SmsMessage>, ISmsMessageService
    {
        private readonly IMapper _mapper;
        public SmsMessageService(IMapper mapper, IAsyncRepository<SmsMessage> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        private IQueryable<SmsMessage> GetQueryable(SmsMessagePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            return query;
        }

        public async Task<PagedResult2<SmsMessageBasic>> GetPaged(SmsMessagePaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SmsMessageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SmsMessageBasic>>(items)
            };
        }
    }
}
