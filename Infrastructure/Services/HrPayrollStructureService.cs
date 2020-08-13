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
            var res = await SearchQuery(x=>x.Id == Id).Include(x => x.Rules).Include("Rules.Category").FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayrollStructureDisplay>> GetPaged(HrPayrollStructurePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Filter))
            {
                query = query.Where(x => x.Name.Contains(val.Filter));
            }
            query = query.Include(x => x.Rules).Include("Rules.Category");

            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayrollStructureDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrPayrollStructureDisplay>>(items)
            };
        }
    }
}
