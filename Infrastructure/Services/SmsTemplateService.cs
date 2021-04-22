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
    public class SmsTemplateService : BaseService<SmsTemplate>, ISmsTemplateService
    {
        private readonly IMapper _mapper;
        public SmsTemplateService(IMapper mapper, IAsyncRepository<SmsTemplate> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SmsTemplateBasic>> GetPaged(SmsTemplatePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SmsTemplateBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SmsTemplateBasic>>(items)
            };
        }
    }
}
