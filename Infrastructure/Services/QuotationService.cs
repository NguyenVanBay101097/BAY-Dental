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
    public class QuotationService : BaseService<Quotation>, IQuotationService
    {
        private readonly IMapper _mapper;

        public QuotationService(IAsyncRepository<Quotation> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuotationDisplay>> GetDisplay(Guid id)
        {
            var model = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.User)
                .Include(x => x.Lines).ToListAsync();
            return _mapper.Map<IEnumerable<QuotationDisplay>>(model);
        }

        public async Task<PagedResult2<QuotationBasic>> GetPagedResultAsync(QuotationPaged val)
        {
            var query = SearchQuery();
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateQuotation >= val.DateFrom.Value);
            if (val.DateTo.HasValue)
            {
                val.DateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateQuotation <= val.DateTo.Value);
            }
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            var totalItem = await query.CountAsync();
            var items = await query.Include(x => x.Partner).Take(val.Limit).Skip(val.Offset).ToListAsync();
            return new PagedResult2<QuotationBasic>(totalItem, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<QuotationBasic>>(items)
            };
        }
    }
}
