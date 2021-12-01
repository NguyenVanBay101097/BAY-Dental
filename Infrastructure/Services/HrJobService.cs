using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Infrastructure.Data;
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
    public class HrJobService : BaseService<HrJob>, IHrJobService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        public HrJobService(IAsyncRepository<HrJob> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, CatalogDbContext context)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedResult2<HrJobBasic>> GetPagedResultAsync(HrJobPaged val)
        {
            var query = SearchQuery(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if(val.CompanyId.HasValue)
                query = query.Where(x=> x.CompanyId.Value == val.CompanyId.Value);

            var totalItem = await query.CountAsync();
            query = query.OrderByDescending(x => x.DateCreated);
            var items = await _mapper.ProjectTo<HrJobBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            
            return new PagedResult2<HrJobBasic>(totalItem, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrJobBasic>>(items)
            };
        }

        public override ISpecification<HrJob> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "hr.hr_job_comp_rule":
                    return new InitialSpecification<HrJob>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }
    }
}
