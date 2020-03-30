using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class FacebookMassMessagingService : BaseService<FacebookMassMessaging>, IFacebookMassMessagingService
    {
        private readonly IMapper _mapper;

        public FacebookMassMessagingService(IAsyncRepository<FacebookMassMessaging> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<FacebookMassMessagingBasic>> GetPagedResultAsync(FacebookMassMessagingPaged val)
        {
            ISpecification<FacebookMassMessaging> spec = new InitialSpecification<FacebookMassMessaging>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<FacebookMassMessaging>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<FacebookMassMessagingBasic>(query).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<FacebookMassMessagingBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
