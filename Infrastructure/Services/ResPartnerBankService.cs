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
    public class ResPartnerBankService : BaseService<ResPartnerBank>, IResPartnerBankService
    {
        private readonly IMapper _mapper;
        public ResPartnerBankService(IAsyncRepository<ResPartnerBank> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ResPartnerBankBasic>> GetPagedResultAsync(ResPartnerBankPaged paged)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(paged.Search))
                query = query.Where(x => x.AccountNumber.Contains(paged.Search) || x.Partner.Name.Contains(paged.Search) || x.Bank.Name.Contains(paged.Search));

            if (paged.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId.Equals(paged.PartnerId));

            if (paged.BankId.HasValue)
                query = query.Where(x => x.BankId.Equals(paged.BankId));

            var items = await query.OrderBy(x => x.DateCreated).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<ResPartnerBankBasic>(totalItems, paged.Offset, paged.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResPartnerBankBasic>>(items)
            };
        }
    }
}
