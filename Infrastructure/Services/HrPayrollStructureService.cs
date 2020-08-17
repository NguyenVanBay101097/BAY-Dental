using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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

        public async Task<HrPayrollStructure> GetHrPayrollStructureDisplay(Guid Id)
        {
            var res = await SearchQuery(x=>x.Id == Id).Include(x => x.Rules).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayrollStructureDisplay>> GetPaged(HrPayrollStructurePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Filter))
            {
                query = query.Where(x => x.Name.Contains(val.Filter));
            }
            query = query.Include(x => x.Rules).Include("Rules.Company");

            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayrollStructureDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrPayrollStructureDisplay>>(items)
            };
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

        public async Task SaveRules(HrPayrollStructureSave val, HrPayrollStructure structure)
        {
            var rulesToRemove = new List<HrSalaryRule>();
            foreach (var rule in structure.Rules)
            {
                if (!val.Rules.Any(x => x.Id == rule.Id))
                    rulesToRemove.Add(rule);
            }

            await GetService<IHrSalaryRuleService>().Remove(rulesToRemove.Select(x=>x.Id).ToList());
            structure.Rules = structure.Rules.AsList().Except(rulesToRemove).ToList();

            foreach (var rule in val.Rules)
            {
                if (rule.Id == Guid.Empty || !rule.Id.HasValue)
                {
                    var r = _mapper.Map<HrSalaryRule>(rule);
                    r.CompanyId = CompanyId;
                    structure.Rules.Add(r);
                }
                else
                {
                    _mapper.Map(rule, structure.Rules.SingleOrDefault(c => c.Id == rule.Id));
                }
            }
        }
    }
}
