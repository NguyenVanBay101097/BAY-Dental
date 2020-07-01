﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TCareScenarioService : BaseService<TCareScenario>, ITCareScenarioService
    {
        private readonly IMapper _mapper;
        public TCareScenarioService(IAsyncRepository<TCareScenario> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public Task<TCareScenario> GetDefault()
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult2<TCareScenarioBasic>> GetPagedResultAsync(TCareScenarioPaged val)
        {
            ISpecification<TCareScenario> spec = new InitialSpecification<TCareScenario>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<TCareScenario>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<TCareScenarioBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<TCareScenarioBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<TCareScenarioDisplay> GetDisplay(Guid id)
        {
            var result = await SearchQuery(x => x.Id == id).Include(x => x.Campaigns).FirstOrDefaultAsync();
            var res = _mapper.Map<TCareScenarioDisplay>(result);
            return res;
        }
    }
}
