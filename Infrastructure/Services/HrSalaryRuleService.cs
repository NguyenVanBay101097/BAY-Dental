using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
   public class HrSalaryRuleService : BaseService<HrSalaryRule>, IHrSalaryRuleService
    {

        private readonly IMapper _mapper;
        public HrSalaryRuleService(IAsyncRepository<HrSalaryRule> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task Remove(IEnumerable<Guid> Ids)
        {
            var list = await SearchQuery(x=>Ids.Contains(x.Id)).ToListAsync();
            await DeleteAsync(list);
        }

        public async Task<HrSalaryRule> GetHrSalaryRuleDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrSalaryRuleDisplay>> GetPaged(HrSalaryRulePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Filter))
            {
                query = query.Where(x => x.Name.Contains(val.Filter));
            }
            if (val.StructId != null)
            {
                query = query.Where(x=>x.StructId == val.StructId);
            }

            var items = await query.ToListAsync();
            //var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrSalaryRuleDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrSalaryRuleDisplay>>(items)
            };
        }
    }
}
