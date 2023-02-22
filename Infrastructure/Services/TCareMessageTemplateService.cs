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
    public class TCareMessageTemplateService : BaseService<TCareMessageTemplate>, ITCareMessageTemplateService
    {
        public readonly IMapper _mapper;
        public TCareMessageTemplateService(IAsyncRepository<TCareMessageTemplate> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<TCareMessageTemplateDisplay> GetDisplay(Guid id)
        {
            var res = await _mapper.ProjectTo<TCareMessageTemplateDisplay>(SearchQuery(x=>x.Id == id).Include(x=>x.CouponProgram)).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<TCareMessageTemplateBasic>> GetPaged(TCareMessageTemplatePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }

            var items = await _mapper.ProjectTo<TCareMessageTemplateBasic>(query.Skip(val.Offset).Take(val.Limit).OrderByDescending(x=>x.DateCreated)).ToListAsync();
            var total = await query.CountAsync();

            return new PagedResult2<TCareMessageTemplateBasic>(total, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
