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
    public class HrPayrollStructureTypeService : BaseService<HrPayrollStructureType>, IHrPayrollStructureTypeService
    {

        private readonly IMapper _mapper;
        public HrPayrollStructureTypeService(IAsyncRepository<HrPayrollStructureType> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<HrPayrollStructureType> GetHrPayrollStructureTypeDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).Include(x => x.DefaultStruct).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayrollStructureTypeDisplay>> GetPaged(HrPayrollStructureTypePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }
            query = query.Include(x => x.DefaultStruct);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayrollStructureTypeDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrPayrollStructureTypeDisplay>>(items)
            };
        }

        public async Task<IEnumerable<HrPayrollStructureTypeSimple>> GetAutocompleteAsync(HrPayrollStructureTypePaged val)
        {
            ISpecification<HrPayrollStructureType> spec = new InitialSpecification<HrPayrollStructureType>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<HrPayrollStructureType>(x => x.Name.Contains(val.Search)));
            //if (!string.IsNullOrEmpty(val.Type))
            //    spec = spec.And(new InitialSpecification<PartnerSource>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<HrPayrollStructureTypeSimple>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();

            return items;
        }

        public override ISpecification<HrPayrollStructureType> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "hr.payroll_structure_type_comp_rule":
                    return new InitialSpecification<HrPayrollStructureType>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }
    }
}
