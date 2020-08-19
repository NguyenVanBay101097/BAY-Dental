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
            var res = await SearchQuery(x=>x.Id == Id).Include(x => x.DefaultStruct).FirstOrDefaultAsync();
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

            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayrollStructureTypeDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrPayrollStructureTypeDisplay>>(items)
            };
        }
    }
}
