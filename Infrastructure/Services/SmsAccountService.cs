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
    public class SmsAccountService : BaseService<SmsAccount>, ISmsAccountService
    {
        private readonly IMapper _mapper;
        public SmsAccountService(IMapper mapper, IAsyncRepository<SmsAccount> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<SmsAccountBasic> GetDefault()
        {
            var smsAccount = await SearchQuery(x => x.CompanyId == CompanyId).OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
            return _mapper.Map<SmsAccountBasic>(smsAccount);
        }

        public async Task<PagedResult2<SmsAccountBasic>> GetPaged(SmsAccountPaged val)
        {
            var query = SearchQuery(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.BrandName.Contains(val.Search));
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SmsAccountBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SmsAccountBasic>>(items)
            };
        }

        public async Task<IEnumerable<SmsSupplierBasic>> SmsSupplierAutocomplete(string search)
        {
            var query = SearchQuery(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.Name.Contains(search));
            var items = await query.GroupBy(x => new { Provider = x.Provider, Name = x.Name }).Select(x => new SmsSupplierBasic
            {
                Name = x.Key.Name,
                Provider = x.Key.Provider
            }).OrderBy(x => x.Name).ToListAsync();

            return items;
        }
    }
}
