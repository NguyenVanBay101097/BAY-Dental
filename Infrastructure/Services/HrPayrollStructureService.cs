using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Dapper;
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
    public class HrPayrollStructureService : BaseService<HrPayrollStructure>, IHrPayrollStructureService
    {

        private readonly IMapper _mapper;
        public HrPayrollStructureService(IAsyncRepository<HrPayrollStructure> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<HrPayrollStructureBase> GetFirstOrDefault(Guid typeId)
        {
            var res = await SearchQuery(x => x.TypeId == typeId).FirstOrDefaultAsync();
            return _mapper.Map<HrPayrollStructureBase>(res);
        }

        public async Task<HrPayrollStructureDisplay> GetHrPayrollStructureDisplay(Guid Id)
        {
            var res = await _mapper.ProjectTo<HrPayrollStructureDisplay>(SearchQuery(x => x.Id == Id)).FirstOrDefaultAsync();
            var ruleObj = GetService<IHrSalaryRuleService>();
            res.Rules = await _mapper.ProjectTo<HrSalaryRuleDisplay>(ruleObj.SearchQuery(x => x.StructId == Id, orderBy: x => x.OrderBy(s => s.Sequence))).ToListAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayrollStructureBasic>> GetPaged(HrPayrollStructurePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Filter))
            {
                query = query.Where(x => x.Name.Contains(val.Filter));
            }
            if (val.StructureTypeId.HasValue)
            {

                query = query.Where(x => x.TypeId == val.StructureTypeId);
            }
            query = query.Include(x => x.Rules).Include("Rules.Company").Include(x => x.Type);

            var items = await _mapper.ProjectTo<HrPayrollStructureBasic>(query.Skip(val.Offset).Take(val.Limit).OrderByDescending(x => x.DateCreated)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayrollStructureBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<HrSalaryRuleDisplay>> GetRules(Guid structureId)
        {
            var res = await GetService<IHrSalaryRuleService>().SearchQuery(x => x.StructId == structureId).ToListAsync();
            return _mapper.Map<IEnumerable<HrSalaryRuleDisplay>>(res);
        }

        public async Task Remove(Guid Id)
        {
            var HrPayrollStructure = await SearchQuery(x => x.Id == Id).Include(x => x.Rules).FirstOrDefaultAsync();
            if (HrPayrollStructure == null)
            {
                throw new Exception("không tìm thấy!");
            }
            await GetService<IHrSalaryRuleService>().DeleteAsync(HrPayrollStructure.Rules);

            await DeleteAsync(HrPayrollStructure);
        }

        public async Task<HrPayrollStructureBasic> ExistRegular(Guid typeId, Guid currentId)
        {
            return await _mapper.ProjectTo<HrPayrollStructureBasic>(SearchQuery(x => x.TypeId == typeId && x.RegularPay == true && x.Id != currentId)).FirstOrDefaultAsync();
        }

        public override ISpecification<HrPayrollStructure> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "hr.payroll_structure_comp_rule":
                    return new InitialSpecification<HrPayrollStructure>(x => !x.CompanyId.HasValue || companyIds.Contains(x.CompanyId.Value));
                default:
                    return null;
            }
        }
    }
}
